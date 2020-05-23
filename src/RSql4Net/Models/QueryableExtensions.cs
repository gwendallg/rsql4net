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
        /// <param name="pageable"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IPage<T> Page<T>(this IQueryable<T> obj, IPageable<T> pageable) where T : class
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (pageable == null) throw new ArgumentNullException(nameof(pageable));

            if (pageable.Sort()?.OrderBy?.Count() > 0)
            {
                obj = pageable.Sort().OrderBy.Aggregate(obj, (current, order) => current.OrderBy(order));
            }

            if (pageable.Sort()?.OrderDescendingBy?.Count() > 0)
            {
                obj = pageable.Sort().OrderDescendingBy.Aggregate(obj,
                    (current, order) => current.OrderByDescending(order));
            }

            var result = obj.ToList();

            if (pageable == null || obj.Count() <= pageable.PageSize())
            {
                return new Page<T>(result);
            }

            var offset = pageable.PageNumber() * pageable.PageSize();
            var limit = pageable.PageSize();
            result = result.Skip(offset)
                .Take(limit)
                .ToList();

            return new Page<T>(result, pageable, obj.Count());
        }
    }
}
