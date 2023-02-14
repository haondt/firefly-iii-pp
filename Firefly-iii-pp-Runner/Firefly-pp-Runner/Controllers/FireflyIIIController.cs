using Firefly_iii_pp_Runner.ExceptionFilters;
using Firefly_iii_pp_Runner.Exceptions;
using Firefly_iii_pp_Runner.Services;
using FireflyIIIpp.FireflyIII.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Firefly_iii_pp_Runner.Controllers
{
    [Route("api/v1/firefly_iii")]
    public class FireflyIIIController : BaseController
    {
        private readonly IFireflyIIIService _fireflyIII;

        public FireflyIIIController(IFireflyIIIService fireflyIII)
        {
            _fireflyIII = fireflyIII;
        }

        [HttpGet]
        [Route("transactions/{id}")]
        public async Task<IActionResult> GetTransaction(string id)
        {
            var transaction = await _fireflyIII.GetTransaction(id);
            if (transaction.Attributes.Transactions.Count != 1)
                return new BadRequestObjectResult("Transaction contains multiple splits") ;
            var transactionData = transaction.Attributes.Transactions[0];
            return new OkObjectResult(transactionData);
        }
    }
}
