using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firefly_iii_pp_Runner.Controllers;
using Firefly_iii_pp_Runner.Services;
using Firefly_pp_Runner.Models.NodeRed;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Firefly_pp_Runner.Controllers
{
    [Route("api/v1/node-red")]
    public class NodeRedController : BaseController
    {
        private readonly NodeRedService _nodeRed;

        public NodeRedController(NodeRedService nodeRed)
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

            var result = await _nodeRed.ApplyRules(dto.StringifiedJsonPayload);
            return new OkObjectResult(new NodeRedPassthroughResponseDto
            {
                StringifiedJsonPayload = result
            });
        }
    }
}
