using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using RSql4Net.Models;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Queries;

namespace RSql4Net.Controllers
{
    /// <summary>
    /// the Controller base extensions
    /// </summary>
    public static class ControllerBaseExtensions
    {
        /// <summary>
        /// filter object list and convert result to RSql page
        /// and return :
        ///  200 OK : if the filtered data is contained in one unique page
        ///  206 Partial : if the filtered data is contained in several pages
        /// </summary>
        /// <param name="controllerBase"></param>
        /// <param name="obj"></param>
        /// <param name="pageable"></param>
        /// <param name="query"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IActionResult Page<T>(this ControllerBase controllerBase, 
            IQueryable<T> obj,
            IRSqlPageable<T> pageable,
            IRSqlQuery<T> query = null)
            where T : class
        {
            if (controllerBase == null)
            {
                throw new ArgumentNullException(nameof(controllerBase));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (pageable == null)
            {
                throw new ArgumentNullException(nameof(pageable));
            }

            var data = query == null ? obj : obj.Where(query.Value());
            var page = data.Page(pageable);
            var statusCode =
                (int)((page.TotalElements != page.NumberOfElements) ? HttpStatusCode.PartialContent : HttpStatusCode.OK);
            return controllerBase.StatusCode(statusCode, page);
        }
    }
}
