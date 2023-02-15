using Firefly_iii_pp_Runner.ExceptionFilters;
using Firefly_iii_pp_Runner.Models;
using Firefly_iii_pp_Runner.Services;
using Firefly_pp_Runner.Models.Runner.Dtos;
using FireflyIIIpp.FireflyIII.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Mime;

namespace Firefly_iii_pp_Runner.Controllers
{
    [Route("api/v1/[controller]")]
    public class RunnerController : BaseController
    {
        private readonly ILogger<RunnerController> _logger;
        private readonly JobManager _jobManager;
        private readonly IFireflyIIIService _fireflyIIIService;

        public RunnerController(ILogger<RunnerController> logger, JobManager jobManager, IFireflyIIIService fireflyIIIService)
        {
            _logger = logger;
            _jobManager = jobManager;
            _fireflyIIIService = fireflyIIIService;
        }

        [HttpPost]
        [Route("start")]
        public async Task<IActionResult> StartJob([FromBody] RunnerDto dto)
        {
            return new OkObjectResult(await _jobManager.StartJob(dto));
        }

        [HttpPost]
        [Route("query/start")]
        public async Task<IActionResult> StartQueryJob([FromBody] QueryStartJobRequestDto dto)
        {
            return new OkObjectResult(await _jobManager.StartJob(dto));
        }

        [HttpPost]
        [Route("query/dry-run")]
        public async Task<IActionResult> DryRunJob([FromBody] QueryStartJobRequestDto dto)
        {
            var query = _fireflyIIIService.PrepareQuery(dto.Operations);
            var container = await _fireflyIIIService.GetTransactions(dto.Operations, 1);
            var sample = container.Data.FirstOrDefault(c => c.Attributes.Transactions.Count > 0)
                ?.Attributes.Transactions.First();

            return new OkObjectResult(new DryRunResponseDto
            {
                Operations = dto.Operations,
                Query = query,
                TotalTransactions = container.Meta.Pagination.Total,
                TotalPages = container.Meta.Pagination.Total_pages,
                Sample = sample
            });
        }

        [HttpPost]
        [Route("single")]
        public async Task<IActionResult> StartSingleJob([FromBody] RunnerSingleDto dto)
        {
            return new OkObjectResult(await _jobManager.StartSingle(dto));
        }

        [HttpPost]
        [Route("stop")]
        public IActionResult StopJob()
        {
            return new OkObjectResult(_jobManager.StopJob());
        }

        [HttpGet]
        [Route("status")]
        public IActionResult GetStatus()
        {
            return new OkObjectResult(_jobManager.GetStatus());
        }
    }
}
