using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RSql4Net.Models.Paging
{
    /// <summary>
    ///     Sort expression.
    /// </summary>
    public class RSqlSort<T> where T : class
    {
        /// <summary>
        ///     Initializes a new instance of.
        /// </summary>
        /// <param name="orderBy">Order by.</param>
        /// <param name="orderDescendingBy">Order descending by.</param>
        public RSqlSort(IEnumerable<Expression<Func<T, object>>> orderBy = null,
            IEnumerable<Expression<Func<T, object>>> orderDescendingBy = null)
        {
            OrderBy = orderBy;
            OrderDescendingBy = orderDescendingBy;
        }

        /// <summary>
        ///     Gets the order by.
        /// </summary>
        /// <value>The order by.</value>
        public IEnumerable<Expression<Func<T, object>>> OrderBy { get; }

        /// <summary>
        ///     Gets the order descending by.
        /// </summary>
        /// <value>The order descending by.</value>
        public IEnumerable<Expression<Func<T, object>>> OrderDescendingBy { get; }
    }
}
