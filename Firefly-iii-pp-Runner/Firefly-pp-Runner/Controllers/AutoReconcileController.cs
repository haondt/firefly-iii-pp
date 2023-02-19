using Firefly_iii_pp_Runner.Controllers;
using Firefly_pp_Runner.Services;
using FireflyIIIpp.FireflyIII.Abstractions;
using FireflyIIIppRunner.Abstractions;
using FireflyIIIppRunner.Abstractions.AutoReconcile;
using FireflyIIIppRunner.Abstractions.AutoReconcile.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Controllers
{
    [Route("api/v1/auto-reconcile")]
    public class AutoReconcileController : BaseController
    {
        private readonly IAutoReconcileService _reconcileService;

        public AutoReconcileController(IAutoReconcileService reconcileService)
        {
            _reconcileService = reconcileService;
        }

        [HttpPost]
        [Route("dry-run")]
        public async Task<IActionResult> StartDryRunJob([FromBody] AutoReconcileRequestDto dto)
        {
            return new OkObjectResult(await _reconcileService.DryRun(dto));
        }

        [HttpPost]
        [Route("run")]
        public async Task<IActionResult> StartJob([FromBody] AutoReconcileRequestDto dto)
        {
            return new OkObjectResult(await _reconcileService.Run(dto));
        }

        [HttpGet]
        [Route("status")]
        public IActionResult GetStatus()
        {
            return new OkObjectResult(_reconcileService.GetStatus());
        }

        [HttpGet]
        [Route("dry-run")]
        public async Task<IActionResult> GetDryRunResult()
        {
            return new OkObjectResult(await _reconcileService.GetDryRunResult());
        }

        [HttpPost]
        [Route("stop")]
        public IActionResult StopJob()
        {
            return new OkObjectResult(_reconcileService.Stop());
        }

    }
}
