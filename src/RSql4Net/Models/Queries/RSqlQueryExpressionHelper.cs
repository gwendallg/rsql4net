using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using RSql4Net.Models.Queries.Exceptions;

namespace RSql4Net.Models.Queries
{
    public static class RSqlQueryExpressionHelper
    {
        
        private static readonly IList<Type> LowerOrGreaterComparisonTypes = new List<Type>
        {
            typeof(short),
            typeof(short?),
            typeof(int),
            typeof(int?),
            typeof(long),
            typeof(long?),
            typeof(float),
            typeof(float?),
            typeof(double),
            typeof(double?),
            typeof(decimal),
            typeof(decimal?),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?),
            typeof(char),
            typeof(char?),
            typeof(byte),
            typeof(byte?)
        };

        private static readonly IList<Type> EqualComparisonTypes = new List<Type>(LowerOrGreaterComparisonTypes)
        {
            typeof(string),
            typeof(bool), 
            typeof(bool?), 
            typeof(Guid), 
            typeof(Guid?)
        };

        private static readonly string MaskLk = $"[{Guid.NewGuid().ToString()}]";

        /// <summary>
        /// 42 is a true response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        #region GetExpression

        /// <summary>
        ///     create and expression ( operator ";" )
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetAndExpression<T>(
            IRSqlQueryVisitor<Expression<Func<T, bool>>> visitor, RSqlQueryParser.AndContext context)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var right = context.constraint()[0].Accept(visitor);
            if (context.constraint().Length == 1)
            {
                return right;
            }

            for (var i = 1; i < context.constraint().Length; i++)
            {
                var left = context.constraint()[i].Accept(visitor);
                right = Expression.Lambda<Func<T, bool>>(Expression.And(left.Body, right.Body), left.Parameters);
            }

            return right;
        }

        /// <summary>
        ///     create or expression ( operator "," )
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetOrExpression<T>(
            IRSqlQueryVisitor<Expression<Func<T, bool>>> visitor, RSqlQueryParser.OrContext context)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var right = context.and()[0].Accept(visitor);
            if (context.and().Length == 1)
            {
                return right;
            }

            for (var i = 1; i < context.and().Length; i++)
            {
                var left = context.and()[i].Accept(visitor);
                right = Expression.Lambda<Func<T, bool>>(Expression.Or(left.Body, right.Body), left.Parameters);
            }

            return right;
        }

        /// <summary>
        ///     create is null expression ( operator "=is-null=" or "=nil=" )
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="jsonNamingPolicy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetIsNullExpression<T>(ParameterExpression parameter,
            RSqlQueryParser.ComparisonContext context,
            JsonNamingPolicy jsonNamingPolicy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, jsonNamingPolicy);
            if (expressionValue.Property.PropertyType.IsValueType &&
                !(expressionValue.Property.PropertyType.IsGenericType &&
                  expressionValue.Property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                throw new ComparisonInvalidComparatorSelectionException(context);
            }

            var values = RSqlQueryGetValueHelper.GetValues(typeof(bool), context.arguments());
            if (!values.Any())
            {
                throw new ComparisonNotEnoughArgumentException(context);
            }

            if (values.Count > 1)
            {
                throw new ComparisonTooManyArgumentException(context);
            }

            var result = Expression.Lambda<Func<T, bool>>(Expression.Equal(
                expressionValue.Expression,
                Expression.Constant(null, typeof(object))), parameter);
            if ((bool)values[0])
            {
                return result;
            }

            var body = Expression.Not(result.Body);
            result = Expression.Lambda<Func<T, bool>>(body, parameter);
            return result;
        }
        
        /// <summary>
        /// extract the unique value
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="expressionValue"></param>
        /// <param name="context"></param>
        /// <param name="jsonNamingPolicy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ComparisonNotEnoughArgumentException"></exception>
        /// <exception cref="ComparisonTooManyArgumentException"></exception>
        private static object GetUniqueValue<T>(ParameterExpression parameter, ExpressionValue expressionValue,
            RSqlQueryParser.ComparisonContext context,
            JsonNamingPolicy jsonNamingPolicy = null)
        {
            var value = context.arguments().value();
            if (value.Length == 0)
            {
                throw new ComparisonNotEnoughArgumentException(context);
            }

            if (value.Length > 1)
            {
                throw new ComparisonTooManyArgumentException(context);
            }

            return RSqlQueryGetValueHelper.GetValue<T>(parameter, expressionValue, context, jsonNamingPolicy);
        }

        /// <summary>
        /// extract the multi value
        /// </summary>
        /// <param name="expressionValue"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ComparisonNotEnoughArgumentException"></exception>
        private static List<object> GetMultipleValue(ExpressionValue expressionValue,
            RSqlQueryParser.ComparisonContext context)
        {
            var value = context.arguments().value();
            if (value.Length == 0)
            {
                throw new ComparisonNotEnoughArgumentException(context);
            }

            return RSqlQueryGetValueHelper.GetValues(expressionValue.Property.PropertyType, context.arguments());
        }

        /// <summary>
        ///     create equal expression ( operator "==" or "=eq=" )
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="jsonNamingPolicy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetEqExpression<T>(ParameterExpression parameter,
            RSqlQueryParser.ComparisonContext context,
            JsonNamingPolicy jsonNamingPolicy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, jsonNamingPolicy);
            if (expressionValue == null)
            {
                throw new ComparisonUnknownSelectorException(context);
            }
            if (!EqualComparisonTypes.Contains(expressionValue.Property.PropertyType) &&
                (!expressionValue.Property.PropertyType.IsEnum &&
                 !(expressionValue.Property.PropertyType.IsGenericType &&
                   expressionValue.Property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                   expressionValue.Property.PropertyType.GetGenericArguments()[0].IsEnum)))
            {
                throw new ComparisonInvalidComparatorSelectionException(context);
            }

            var value = GetUniqueValue<T>(parameter, expressionValue, context, jsonNamingPolicy);
            var expression = value is ExpressionValue valueExp1
                ? valueExp1.Expression
                : Expression.Constant(value, expressionValue.Property.PropertyType);
            if (value is ExpressionValue valueExp2 &&
                valueExp2.Property.PropertyType != expressionValue.Property.PropertyType)
            {
                throw new ComparisonInvalidMatchTypeException(context);
            }

            if (expressionValue.Property.PropertyType != typeof(string) || value is ExpressionValue)
            {
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(
                    expressionValue.Expression, expression
                ), parameter);
            }

            var v = ((string)value).Replace(@"\*", MaskLk);
            if (v.IndexOf('*') != -1)
            {
                return GetLkExpression<T>(parameter, context, jsonNamingPolicy);
            }

            value = v.Replace(MaskLk, "*");

            return Expression.Lambda<Func<T, bool>>(Expression.Equal(
                expressionValue.Expression,
                Expression.Constant(value, expressionValue.Property.PropertyType)), parameter);
        }

        /// <summary>
        ///     create neq expression ( operator "!=" or "=neq=" )
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="jsonNamingPolicy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetNeqExpression<T>(ParameterExpression parameter,
            RSqlQueryParser.ComparisonContext context,
            JsonNamingPolicy jsonNamingPolicy = null)
        {
            if (parameter == null)
            {
                throw new ArgumentException(nameof(parameter));
            }

            if (context == null)
            {
                throw new ArgumentException(nameof(context));
            }

            var expression = GetEqExpression<T>(parameter, context, jsonNamingPolicy);
            var body = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        /// <summary>
        ///     create less than expression ( operator "&gt;" or "=lt=" )
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="jsonNamingPolicy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetLtExpression<T>(ParameterExpression parameter,
            RSqlQueryParser.ComparisonContext context,
            JsonNamingPolicy jsonNamingPolicy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, jsonNamingPolicy);
            if (!LowerOrGreaterComparisonTypes.Contains(expressionValue.Property.PropertyType))
            {
                throw new ComparisonInvalidComparatorSelectionException(context);
            }

            var value = GetUniqueValue<T>(parameter, expressionValue, context, jsonNamingPolicy);
            var expression = value is ExpressionValue valueExp1
                ? valueExp1.Expression
                : Expression.Constant(value, expressionValue.Property.PropertyType);
            if (value is ExpressionValue valueExp2 &&
                valueExp2.Property.PropertyType != expressionValue.Property.PropertyType)
            {
                throw new ComparisonInvalidMatchTypeException(context);
            }

            return Expression.Lambda<Func<T, bool>>(Expression.LessThan(
                expressionValue.Expression,
                expression), parameter);
        }

        /// <summary>
        ///     create less than or equal expression ( operator "&lt;=" or "=le=" )
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="jsonNamingPolicy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetLeExpression<T>(ParameterExpression parameter,
            RSqlQueryParser.ComparisonContext context,
            JsonNamingPolicy jsonNamingPolicy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, jsonNamingPolicy);
            if (!LowerOrGreaterComparisonTypes.Contains(expressionValue.Property.PropertyType))
            {
                throw new ComparisonInvalidComparatorSelectionException(context);
            }

            var value = GetUniqueValue<T>(parameter, expressionValue, context, jsonNamingPolicy);
            var expression = value is ExpressionValue valueExp1
                ? valueExp1.Expression
                : Expression.Constant(value, expressionValue.Property.PropertyType);
            if (value is ExpressionValue valueExp2 &&
                valueExp2.Property.PropertyType != expressionValue.Property.PropertyType)
            {
                throw new ComparisonInvalidMatchTypeException(context);
            }

            return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(
                expressionValue.Expression,
                expression), parameter);
        }

        /// <summary>
        ///     create greater than expression ( operator ">" or "=gt=" )
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="jsonNamingPolicy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetGtExpression<T>(ParameterExpression parameter,
            RSqlQueryParser.ComparisonContext context,
            JsonNamingPolicy jsonNamingPolicy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, jsonNamingPolicy);
            if (!LowerOrGreaterComparisonTypes.Contains(expressionValue.Property.PropertyType))
            {
                throw new ComparisonInvalidComparatorSelectionException(context);
            }

            var value = GetUniqueValue<T>(parameter, expressionValue, context, jsonNamingPolicy);
            var expression = value is ExpressionValue valueExp1
                ? valueExp1.Expression
                : Expression.Constant(value, expressionValue.Property.PropertyType);
            if (value is ExpressionValue valueExp2 &&
                valueExp2.Property.PropertyType != expressionValue.Property.PropertyType)
            {
                throw new ComparisonInvalidMatchTypeException(context);
            }

            return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(
                expressionValue.Expression,
                expression), parameter);
        }

        /// <summary>
        /// extract the selector
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="jsonNamingPolicy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static ExpressionValue GetSelector<T>(ParameterExpression parameter,
            RSqlQueryParser.ComparisonContext context,
            JsonNamingPolicy jsonNamingPolicy)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var result = ExpressionValue.Parse<T>(parameter, context.selector().GetText(), jsonNamingPolicy);
            return result ?? throw new ComparisonUnknownSelectorException(context);
        }

        /// <summary>
        ///     create greater than or equal expression ( operator ">=" or "=ge=" )
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="jsonNamingPolicy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetGeExpression<T>(ParameterExpression parameter,
            RSqlQueryParser.ComparisonContext context,
            JsonNamingPolicy jsonNamingPolicy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, jsonNamingPolicy);
            if (!LowerOrGreaterComparisonTypes.Contains(expressionValue.Property.PropertyType))
            {
                throw new ComparisonInvalidComparatorSelectionException(context);
            }

            var value = GetUniqueValue<T>(parameter, expressionValue, context, jsonNamingPolicy);

            var expression = value is ExpressionValue valueExp1
                ? valueExp1.Expression
                : Expression.Constant(value, expressionValue.Property.PropertyType);
            if (value is ExpressionValue valueExp2 &&
                valueExp2.Property.PropertyType != expressionValue.Property.PropertyType)
            {
                throw new ComparisonInvalidMatchTypeException(context);
            }

            return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(
                expressionValue.Expression,
                expression), parameter);
        }

        /// <summary>
        ///     create like expression
        /// </summary>
        /// <returns></returns>
        private static Expression<Func<T, bool>> GetLkExpression<T>(ParameterExpression parameter,
            RSqlQueryParser.ComparisonContext context,
            JsonNamingPolicy jsonNamingPolicy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, jsonNamingPolicy);
            if (expressionValue.Property.PropertyType != typeof(string))
            {
                throw new ComparisonInvalidComparatorSelectionException(context);
            }

            var values = RSqlQueryGetValueHelper.GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (!values.Any())
            {
                throw new ComparisonNotEnoughArgumentException(context);
            }

            if (values.Count > 1)
            {
                throw new ComparisonTooManyArgumentException(context);
            }

            var criteria = Convert.ToString(values[0]);
            var maskStar = "{" + Guid.NewGuid() + "}";
            criteria = criteria.Replace(@"\*", maskStar);
            MethodInfo method;
            if (criteria.IndexOf('*') == -1)
            {
                criteria += '*';
            }

            if (criteria.StartsWith("*", StringComparison.Ordinal) && criteria.EndsWith("*", StringComparison.Ordinal))
            {
                method = QueryReflectionHelper.MethodStringContains;
            }
            else if (criteria.StartsWith("*", StringComparison.Ordinal))
            {
                method = QueryReflectionHelper.MethodStringEndsWith;
            }
            else
            {
                method = QueryReflectionHelper.MethodStringStartsWith;
            }

            criteria = criteria.Replace("*", "").Replace(maskStar, "*");
            return Expression.Lambda<Func<T, bool>>(Expression.Call(expressionValue.Expression,
                method,
                Expression.Constant(criteria, expressionValue.Property.PropertyType)), parameter);
        }

        /// <summary>
        ///     create in expression ( operator "=in=" )
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetInExpression<T>(ParameterExpression parameter,
            RSqlQueryParser.ComparisonContext context,
            JsonNamingPolicy jsonNamingPolicy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, jsonNamingPolicy);
            var values = GetMultipleValue(expressionValue, context);
            var methodContainsInfo =
                QueryReflectionHelper.GetOrRegistryContainsMethodInfo(expressionValue.Property.PropertyType);

            return Expression.Lambda<Func<T, bool>>(
                Expression.Call(Expression.Constant(methodContainsInfo.Convert(values)),
                    methodContainsInfo.ContainsMethod,
                    expressionValue.Expression), parameter);
        }


        /// <summary>
        ///     create not in expression ( operator "=out=" )
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetOutExpression<T>(ParameterExpression parameter,
            RSqlQueryParser.ComparisonContext context,
            JsonNamingPolicy namingStrategy = null)
        {
            if (parameter == null)
            {
                throw new ArgumentException(nameof(parameter));
            }

            if (context == null)
            {
                throw new ArgumentException(nameof(context));
            }

            var expression = GetInExpression<T>(parameter, context, namingStrategy);
            var body = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        #endregion
    }
}
