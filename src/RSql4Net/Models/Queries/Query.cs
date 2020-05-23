using System;
using System.Linq.Expressions;

namespace RSql4Net.Models.Queries
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Query<T> : IQuery<T>
    {
        private readonly Expression<Func<T, bool>> _value;

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        public Query(Expression<Func<T, bool>> value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, bool>> Value()
        {
            return _value;
        }
    }
}
