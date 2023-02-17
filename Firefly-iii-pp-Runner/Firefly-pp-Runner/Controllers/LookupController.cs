using Firefly_iii_pp_Runner.Controllers;
using FireflyIIIppRunner.Abstractions.KeyValueStore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Controllers
{
    [Route("api/v1/lookup")]
    public class LookupController : BaseController
    {
        private readonly IKeyValueStoreService _kvService;

        public LookupController(IKeyValueStoreService kvService)
        {
            _kvService = kvService;
        }

        [HttpPost]
        [Route("read")]
        public async Task<IActionResult> ReadFromStorage()
        {
            await _kvService.ReadFromStorage();
            return new OkResult();
        }

        [HttpPost]
        [Route("write")]
        public async Task<IActionResult> WriteToStorage()
        {
            await _kvService.WriteToStorage();
            return new OkResult();
        }

        [HttpGet]
        [Route("values/{value}/keys")]
        public async Task<IActionResult> GetKeys(string value)
        {
            return new OkObjectResult(await _kvService.GetKeys(value));
        }

        [HttpPost]
        [Route("values/{value}/keys/{key}")]
        public async Task<IActionResult> AddKey(string key, string value)
        {
            await _kvService.AddKey(key, value);
            return new OkResult();
        }

        [HttpDelete]
        [Route("keys/{key}")]
        public async Task<IActionResult> DeleteKey(string key)
        {
            await _kvService.DeleteKey(key);
            return new NoContentResult();
        }

        [HttpGet]
        [Route("values/{value}")]
        public async Task<IActionResult> GetValue(string value)
        {
            return new OkObjectResult(await _kvService.GetValue(value));
        }

        [HttpPut]
        [Route("values/{value}")]
        public async Task<IActionResult> UpdateValue(string value, [FromBody] string valueValue)
        {
            await _kvService.UpdateValue(value, valueValue);
            return new OkResult();
        }
        
        [HttpDelete]
        [Route("values/{value}")]
        public async Task<IActionResult> DeleteValue(string value)
        {
            await _kvService.DeleteValue(value);
            return new OkResult();
        }

        [HttpPost]
        [Route("values/autocomplete")]
        public async Task<IActionResult> AutocompleteValue([FromBody] string partialValue)
        {
            return new OkObjectResult(await _kvService.AutocompleteValue(partialValue));
        }
    }
}
