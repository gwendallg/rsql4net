using System;
using System.Linq;
using RSql4Net.Models.Paging;

namespace RSql4Net.Models
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// create page 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="irSqlPageable"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IRSqlPage<T> Page<T>(this IQueryable<T> obj, IRSqlPageable<T> irSqlPageable) where T : class
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (irSqlPageable == null) throw new ArgumentNullException(nameof(irSqlPageable));

            if (irSqlPageable.Sort()?.OrderBy?.Count() > 0)
            {
                obj = irSqlPageable.Sort().OrderBy.Aggregate(obj, (current, order) => current.OrderBy(order));
            }

            if (irSqlPageable.Sort()?.OrderDescendingBy?.Count() > 0)
            {
                obj = irSqlPageable.Sort().OrderDescendingBy.Aggregate(obj,
                    (current, order) => current.OrderByDescending(order));
            }

            var result = obj.ToList();
            var offset = irSqlPageable.PageNumber() * irSqlPageable.PageSize();
            var limit = irSqlPageable.PageSize();
            result = result.Skip(offset)
                .Take(limit)
                .ToList();

            return new RSqlPage<T>(result, irSqlPageable, obj.Count());
        }
    }
}
