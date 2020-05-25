using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RSql4Net.Models;
using RSql4Net.Models.Paging;
using RSql4Net.Models.Queries;
using RSql4Net.Samples.Models;

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
        public IActionResult Get(IRSqlQuery<Customer> irSqlQuery,
            IRSqlPageable<Customer> irSqlPageable)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorModel(ModelState));
            }

            _logger.LogDebug(irSqlQuery?.ToString());

            var content = _customers
                .AsQueryable();

            content = content
                .Where(irSqlQuery?.Value());

            var page = content.Page(irSqlPageable);

            return StatusCode((int)HttpStatusCode.PartialContent, page);
        }
    }
}
