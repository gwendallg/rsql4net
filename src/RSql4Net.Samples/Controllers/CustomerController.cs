using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Queries;
using RSql4Net.Samples.Models;
using RSql4Net.Controllers;
using RSql4Net.Models;

namespace RSql4Net.Samples.Controllers
{
    [Route("customers")]
    public class CustomerController : Controller
    {
        private readonly IList<Customer> _customers;
 
        public CustomerController(IList<Customer> customers)
        {
            _customers = customers;
        }

        /// <summary>
        /// Get resource items by RSql Query and Pageable Query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageable"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromQuery] IRSqlQuery<Customer> query,
            [FromQuery] IRSqlPageable<Customer> pageable)
        {
            // is not valid request
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorModel(ModelState));
            }

            var page = _customers
                .AsQueryable()
                .Page(pageable, query);

            return this.Page(page);
        }
    }
}
