using System;
using System.Linq;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Queries;

namespace RSql4Net.Models
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="query"></param>
        /// <param name="pageable"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IRSqlPage<T> Page<T>(this IQueryable<T> obj,
            IRSqlPageable<T> pageable,
            IRSqlQuery<T> query=null) where T : class
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (pageable == null) throw new ArgumentNullException(nameof(pageable));
            var where = query == null ? RSqlQueryExpressionHelper.True<T>() : query.Value();
            var count = obj.Count(where);
            IOrderedQueryable<T> sorted = null;
            if (pageable.Sort() != null)
            {
                var sort = pageable.Sort();
                sorted = sort.IsDescending ? obj.OrderByDescending(sort.Value) : obj.OrderBy(sort.Value);
                sort = sort.Next;
                while (sort != null)
                {
                    sorted = sort.IsDescending ? sorted.ThenByDescending(sort.Value) : sorted.ThenBy(sort.Value);
                    sort = sort.Next;
                }
            }

            var offset = pageable.PageNumber() * pageable.PageSize();
            var limit = pageable.PageSize();
            var result = (sorted ?? obj).Where(where)
                .Skip(offset)
                .Take(limit)
                .ToList();

            return new RSqlPage<T>(result, pageable, count);
        }

    }
}
