using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using RSql4Net.Models.Queries.Exceptions;

namespace RSql4Net.Models.Queries
{
    public static class QueryExpressionHelper
    {
        private static readonly IList<Type> EqOrNeqOrInOrOutAutorizedType = new List<Type>
        {
            typeof(string),
            typeof(bool),
            typeof(bool?),
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

        private static readonly IList<Type> LtOrGtOrLeOrLeAutorizedType = new List<Type>
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

        private static readonly string MaskLk = $"[{Guid.NewGuid().ToString()}]";

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
            IQueryVisitor<Expression<Func<T, bool>>> visitor, QueryParser.AndContext context)
        {
            if (visitor == null)
            {
                throw new ArgumentException(nameof(visitor));
            }

            if (context == null)
            {
                throw new ArgumentException(nameof(context));
            }

            if (context.constraint().Length == 0)
            {
                return True<T>();
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
            IQueryVisitor<Expression<Func<T, bool>>> visitor, QueryParser.OrContext context)
        {
            if (visitor == null)
            {
                throw new ArgumentException(nameof(visitor));
            }

            if (context == null)
            {
                throw new ArgumentException(nameof(context));
            }

            if (context.and().Length == 0)
            {
                return True<T>();
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
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetIsNullExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType.IsValueType &&
                !(expressionValue.Property.PropertyType.IsGenericType &&
                  expressionValue.Property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            }

            var values = QueryGetValueHelper.GetValues(typeof(bool), context.arguments());
            if (!values.Any())
            {
                throw new QueryComparisonNotEnoughArgumentException(context);
            }

            if (values.Count > 1)
            {
                throw new QueryComparisonTooManyArgumentException(context);
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

        private static object GetUniqueValue<T>(ParameterExpression parameter, ExpressionValue expressionValue,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (expressionValue == null)
            {
                throw new ArgumentNullException(nameof(expressionValue));
            }

            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var value = context.arguments().value();
            if (value.Length == 0)
            {
                throw new QueryComparisonNotEnoughArgumentException(context);
            }

            if (value.Length > 1)
            {
                throw new QueryComparisonTooManyArgumentException(context);
            }

            return QueryGetValueHelper.GetValue<T>(parameter, expressionValue, context, namingStrategy);
        }

        private static List<object> GetMultipleValue(ParameterExpression parameter, ExpressionValue expressionValue,
            QueryParser.ComparisonContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (expressionValue == null)
            {
                throw new ArgumentNullException(nameof(expressionValue));
            }

            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var value = context.arguments().value();
            if (value.Length == 0)
            {
                throw new QueryComparisonNotEnoughArgumentException(context);
            }

            return QueryGetValueHelper.GetValues(expressionValue.Property.PropertyType, context.arguments());
        }

        /// <summary>
        ///     create equal expression ( operator "==" or "=eq=" )
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetEqExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, namingStrategy);
            if (!EqOrNeqOrInOrOutAutorizedType.Contains(expressionValue.Property.PropertyType) &&
                (!expressionValue.Property.PropertyType.IsEnum &&
                 !(expressionValue.Property.PropertyType.IsGenericType &&
                   expressionValue.Property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                   expressionValue.Property.PropertyType.GetGenericArguments()[0].IsEnum)))
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            }

            var value = GetUniqueValue<T>(parameter, expressionValue, context, namingStrategy);
            var expression = value is ExpressionValue valueExp1
                ? valueExp1.Expression
                : Expression.Constant(value, expressionValue.Property.PropertyType);
            if (value is ExpressionValue valueExp2 &&
                valueExp2.Property.PropertyType != expressionValue.Property.PropertyType)
            {
                throw new QueryComparisonInvalidMatchTypeException(context);
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
                return GetLkExpression<T>(parameter, context, namingStrategy);
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
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetNeqExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            if (parameter == null)
            {
                throw new ArgumentException(nameof(parameter));
            }

            if (context == null)
            {
                throw new ArgumentException(nameof(context));
            }

            var expression = GetEqExpression<T>(parameter, context, namingStrategy);
            var body = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        /// <summary>
        ///     create less than expression ( operator "&gt;" or "=lt=" )
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetLtExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, namingStrategy);
            if (!LtOrGtOrLeOrLeAutorizedType.Contains(expressionValue.Property.PropertyType))
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            }

            var value = GetUniqueValue<T>(parameter, expressionValue, context, namingStrategy);
            var expression = value is ExpressionValue valueExp1
                ? valueExp1.Expression
                : Expression.Constant(value, expressionValue.Property.PropertyType);
            if (value is ExpressionValue valueExp2 &&
                valueExp2.Property.PropertyType != expressionValue.Property.PropertyType)
            {
                throw new QueryComparisonInvalidMatchTypeException(context);
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
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetLeExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, namingStrategy);
            if (!LtOrGtOrLeOrLeAutorizedType.Contains(expressionValue.Property.PropertyType))
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            }

            var value = GetUniqueValue<T>(parameter, expressionValue, context, namingStrategy);
            var expression = value is ExpressionValue valueExp1
                ? valueExp1.Expression
                : Expression.Constant(value, expressionValue.Property.PropertyType);
            if (value is ExpressionValue valueExp2 &&
                valueExp2.Property.PropertyType != expressionValue.Property.PropertyType)
            {
                throw new QueryComparisonInvalidMatchTypeException(context);
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
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetGtExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, namingStrategy);
            if (!LtOrGtOrLeOrLeAutorizedType.Contains(expressionValue.Property.PropertyType))
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            }

            var value = GetUniqueValue<T>(parameter, expressionValue, context, namingStrategy);
            var expression = value is ExpressionValue valueExp1
                ? valueExp1.Expression
                : Expression.Constant(value, expressionValue.Property.PropertyType);
            if (value is ExpressionValue valueExp2 &&
                valueExp2.Property.PropertyType != expressionValue.Property.PropertyType)
            {
                throw new QueryComparisonInvalidMatchTypeException(context);
            }

            return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(
                expressionValue.Expression,
                expression), parameter);
        }

        private static ExpressionValue GetSelector<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy)
        {
            if (parameter == null)
            {
                throw new ArgumentException(nameof(parameter));
            }

            if (context == null)
            {
                throw new ArgumentException(nameof(context));
            }

            if (!ExpressionValue.TryParse<T>(parameter, context.selector().GetText(), namingStrategy,
                out var expressionValue))
            {
                throw new QueryComparisonUnknownSelectorException(context);
            }

            return expressionValue;
        }

        /// <summary>
        ///     create greater than or equal expression ( operator ">=" or "=ge=" )
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetGeExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, namingStrategy);
            if (!LtOrGtOrLeOrLeAutorizedType.Contains(expressionValue.Property.PropertyType))
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            }

            var value = GetUniqueValue<T>(parameter, expressionValue, context, namingStrategy);

            var expression = value is ExpressionValue valueExp1
                ? valueExp1.Expression
                : Expression.Constant(value, expressionValue.Property.PropertyType);
            if (value is ExpressionValue valueExp2 &&
                valueExp2.Property.PropertyType != expressionValue.Property.PropertyType)
            {
                throw new QueryComparisonInvalidMatchTypeException(context);
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
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType != typeof(string))
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            }

            var values = QueryGetValueHelper.GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (!values.Any())
            {
                throw new QueryComparisonNotEnoughArgumentException(context);
            }

            if (values.Count > 1)
            {
                throw new QueryComparisonTooManyArgumentException(context);
            }

            var criteria = Convert.ToString(values[0]);
            var maskStar = "{" + Guid.NewGuid() + "}";
            criteria = criteria.Replace(@"\*", maskStar);
            MethodInfo method;
            if (criteria.IndexOf('*') == -1)
            {
                criteria = criteria + '*';
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
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            var expressionValue = GetSelector<T>(parameter, context, namingStrategy);
            var values = GetMultipleValue(parameter, expressionValue, context);
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
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
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
