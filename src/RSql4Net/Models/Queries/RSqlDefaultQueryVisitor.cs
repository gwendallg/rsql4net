using System;
using System.Linq.Expressions;
using System.Text.Json;
using Antlr4.Runtime.Tree;
using RSql4Net.Models.Queries.Exceptions;

namespace RSql4Net.Models.Queries
{
    /// <summary>
    ///     default rsql visitor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RSqlDefaultQueryVisitor<T> : RSqlQueryBaseVisitor<Expression<Func<T, bool>>>
    {
        private readonly JsonNamingPolicy _jsonNamingPolicy;
        private readonly ParameterExpression _parameter;

        /// <summary>
        ///     create instance of object
        /// </summary>
        /// <param name="jsonJsonNamingPolicy"></param>
        public RSqlDefaultQueryVisitor(JsonNamingPolicy jsonJsonNamingPolicy)
        {
            _jsonNamingPolicy = jsonJsonNamingPolicy;
            _parameter = Expression.Parameter(typeof(T));
        }

        /// <summary>
        ///     visit a or expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitOr(RSqlQueryParser.OrContext context)
        {
            return RSqlQueryExpressionHelper.GetOrExpression(this, context);
        }

        /// <summary>
        ///     visit a and expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitAnd(RSqlQueryParser.AndContext context)
        {
            return RSqlQueryExpressionHelper.GetAndExpression(this, context);
        }

        /// <summary>
        ///     visit constraint expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitConstraint(RSqlQueryParser.ConstraintContext context)
        {
            if (context.group() != null)
            {
                return context.group().Accept(this);
            }

            return context.comparison()?.Accept(this);
        }

        /// <summary>
        ///     visit of group expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitGroup(RSqlQueryParser.GroupContext context)
        {
            return context.or()?.Accept(this);
        }

        /// <summary>
        ///     visit of errorNode expression
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitErrorNode(IErrorNode node)
        {
            throw new QueryErrorNodeException(node);
        }

        /// <summary>
        ///     visit a comparison expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitComparison(RSqlQueryParser.ComparisonContext context)
        {
            var comparator = context.comparator().GetText().ToLowerInvariant();
            switch (comparator)
            {
                case "=is-null=":
                case "=nil=":
                    return RSqlQueryExpressionHelper.GetIsNullExpression<T>(_parameter, context, _jsonNamingPolicy);
                case "==":
                case "=eq=":
                    return RSqlQueryExpressionHelper.GetEqExpression<T>(_parameter, context, _jsonNamingPolicy);
                case "!=":
                case "=neq=":
                    return RSqlQueryExpressionHelper.GetNeqExpression<T>(_parameter, context, _jsonNamingPolicy);
                case "<":
                case "=lt=":
                    return RSqlQueryExpressionHelper.GetLtExpression<T>(_parameter, context, _jsonNamingPolicy);
                case "<=":
                case "=le=":
                    return RSqlQueryExpressionHelper.GetLeExpression<T>(_parameter, context, _jsonNamingPolicy);
                case ">":
                case "=gt=":
                    return RSqlQueryExpressionHelper.GetGtExpression<T>(_parameter, context, _jsonNamingPolicy);
                case ">=":
                case "=ge=":
                    return RSqlQueryExpressionHelper.GetGeExpression<T>(_parameter, context, _jsonNamingPolicy);
                case "=in=":
                    return RSqlQueryExpressionHelper.GetInExpression<T>(_parameter, context, _jsonNamingPolicy);
                case "=out=":
                    return RSqlQueryExpressionHelper.GetOutExpression<T>(_parameter, context, _jsonNamingPolicy);
                default:
                    throw new ComparisonUnknownComparatorException(context);
            }
        }
    }
}
