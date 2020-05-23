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
        public IActionResult GetAll(IQuery<Customer> query,
            IPageable<Customer> pageable)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorModel(ModelState));
            }

            _logger.LogDebug(query?.ToString());

            var content = _customers
                .AsQueryable();

            content = content
                .Where(query?.Value());

            var page = content.Page(pageable);

            return StatusCode((int)HttpStatusCode.PartialContent, page);
        }
    }
}
