using Firefly_iii_pp_Runner.API.ExceptionFilters;
using Firefly_iii_pp_Runner.API.Exceptions;
using Firefly_iii_pp_Runner.API.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Firefly_iii_pp_Runner.API.Controllers
{
    [Route("[controller]")]
    public class MongoController : BaseController
    {
        private readonly MongoService _mongoService;

        public MongoController(MongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet]
        [Route("tests/{id}")]
        public async Task<IActionResult> GetTests(string id)
        {
            var tests = await _mongoService.GetTest(id);
            if (tests == null)
                throw new NotFoundException(id);
                //return new NotFoundObjectResult(id);
            return new OkObjectResult(tests);
        }

        [HttpPost]
        [Route("tests/{id}")]
        public async Task<JToken> PostTests(string id, [FromBody] JToken body)
        {
            if (body == null)
                throw new ArgumentException();
            await _mongoService.SetTest(id, body);
            return body;
        }
    }
}
