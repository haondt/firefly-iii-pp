﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firefly_iii_pp_Runner.Controllers;
using Firefly_iii_pp_Runner.Services;
using FireflyIIIpp.NodeRed.Abstractions;
using FireflyIIIpp.NodeRed.Abstractions.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Firefly_pp_Runner.Controllers
{
    [Route("api/v1/node-red")]
    public class NodeRedController : BaseController
    {
        private readonly INodeRedService _nodeRed;

        public NodeRedController(INodeRedService nodeRed)
        {
            _nodeRed = nodeRed;
        }

        [HttpPost]
        [Route("export-flows")]
        public async Task<IActionResult> ExportFlows()
        {
            await _nodeRed.ExportFlows();
            return new OkResult();
        }

        [HttpPost]
        [Route("passthrough")]
        public async Task<IActionResult> Passthrough([FromBody] NodeRedPassthroughRequestDto dto)
        {
            try { JsonConvert.DeserializeObject<Dictionary<string, object>>(dto.StringifiedJsonPayload); }
            catch { throw new ArgumentException("Received invalid JSON object."); }

            var (hasChanges, result) = await _nodeRed.TryApplyRules(dto.StringifiedJsonPayload);
            return new OkObjectResult(new NodeRedPassthroughResponseDto
            {
                StringifiedJsonPayload = hasChanges ? result : dto.StringifiedJsonPayload
            });
        }

        [HttpPost]
        [Route("extract-key/{field}")]
        public async Task<IActionResult> ExtractKey(string field, [FromBody] NodeRedPassthroughRequestDto dto)
        {
            try { JsonConvert.DeserializeObject<Dictionary<string, object>>(dto.StringifiedJsonPayload); }
            catch { throw new ArgumentException("Received invalid JSON object."); }

            var result = await _nodeRed.ExtractKey(field, dto.StringifiedJsonPayload);
            return new OkObjectResult(result);
        }
    }
}
