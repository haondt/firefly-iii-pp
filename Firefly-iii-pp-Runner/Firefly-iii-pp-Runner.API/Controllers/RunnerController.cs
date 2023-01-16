using Firefly_iii_pp_Runner.API.ExceptionFilters;
using Firefly_iii_pp_Runner.API.Exceptions;
using Firefly_iii_pp_Runner.API.Models;
using Firefly_iii_pp_Runner.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Firefly_iii_pp_Runner.API.Controllers
{
    [Route("[controller]")]
    public class RunnerController : BaseController
    {
        private readonly ILogger<RunnerController> _logger;
        private readonly JobManager _jobManager;

        public RunnerController(ILogger<RunnerController> logger, JobManager jobManager)
        {
            _logger = logger;
            _jobManager = jobManager;
        }

        [HttpPost]
        [Route("start")]
        public async Task<IActionResult> StartJob([FromBody] RunnerDto dto)
        {
            return new OkObjectResult(await _jobManager.StartJob(dto));
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
