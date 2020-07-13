using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using RSql4Net.Models.Paging;

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
        /// <param name="page"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IActionResult Page<T>(this ControllerBase controllerBase, 
            IRSqlPage<T> page)
            where T : class
        {
            if (controllerBase == null)
            {
                throw new ArgumentNullException(nameof(controllerBase));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            } 
            var statusCode =
                (int)((page.TotalElements != page.NumberOfElements) ? HttpStatusCode.PartialContent : HttpStatusCode.OK);
            return controllerBase.StatusCode(statusCode, page);
        }
    }
}
