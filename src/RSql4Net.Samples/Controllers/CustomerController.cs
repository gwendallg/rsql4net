using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Queries;
using RSql4Net.Samples.Models;
using RSql4Net.Controllers;

namespace RSql4Net.Samples.Controllers
{
    [Route("customers")]
    public class CustomerController : Controller
    {
        private readonly IList<Customer> _customers;
        private readonly ILogger _logger;

        public CustomerController(IList<Customer> customers, ILoggerFactory loggerFactory)
        {
            _customers = customers;
            _logger = loggerFactory.CreateLogger("CustomerLog");
        }

        [HttpGet]
        public IActionResult Get([FromQuery] IRSqlQuery<Customer> query,
            [FromQuery] IRSqlPageable<Customer> pageable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorModel(ModelState));
            }

            _logger.LogDebug(query?.ToString());

            var content = _customers
                .AsQueryable();

            return this.Page(content, pageable, query);
        }
    }
}
