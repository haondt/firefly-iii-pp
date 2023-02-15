using Firefly_iii_pp_Runner.ExceptionFilters;
using Firefly_iii_pp_Runner.Models.ThunderClient.Dtos;
using Firefly_iii_pp_Runner.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
namespace Firefly_iii_pp_Runner.Controllers
{

    [Route("api/v1/[controller]")]
    public class ThunderController : BaseController
    {
        private readonly ThunderClientEditorService _thunderService;

        public ThunderController(ThunderClientEditorService thunderService)
        {
            _thunderService = thunderService;
        }

        [HttpGet]
        [Route("clientinfo")]
        public async Task<IActionResult> GetClientInfo()
        {
            return new OkObjectResult(new ClientInfoResultDto
            {
                Count = await _thunderService.GetClientCount()
            });
        }

        [HttpPost]
        [Route("postman")]
        public async Task<IActionResult> ImportPostmanFile()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var json = await reader.ReadToEndAsync();
                await _thunderService.ImportPostmanFile(json);
            }

            return new OkResult();
        }

        [HttpPost]
        [Route("sort")]
        public async Task<IActionResult> Sort()
        {
            await _thunderService.SortTests();
            return new OkResult();
        }

        [HttpGet]
        [Route("foldernames")]
        public async Task<IActionResult> GetFolderNames()
        {
            return new OkObjectResult(await _thunderService.GetFolderNames());
        }

        [HttpPost]
        [Route("testcase")]
        public async Task<IActionResult> AddTestCase([FromBody] CreateTestCaseRequestDto request)
        {
            var result = await _thunderService.CreateTestCase(request);
            return new OkObjectResult(new
            {
                Folder = result.Folder,
                Client = result.Client
            });
        }
    }
}
