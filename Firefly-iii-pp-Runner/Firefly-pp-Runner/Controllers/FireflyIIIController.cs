using Firefly_iii_pp_Runner.Controllers;
using FireflyIIIpp.FireflyIII.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Firefly_pp_Runner.Controllers
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
                return new BadRequestObjectResult("Transaction contains multiple splits");
            var transactionData = transaction.Attributes.Transactions[0];
            return new OkObjectResult(transactionData);
        }
    }
}
