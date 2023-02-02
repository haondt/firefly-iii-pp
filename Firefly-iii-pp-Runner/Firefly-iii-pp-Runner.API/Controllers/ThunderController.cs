﻿using Firefly_iii_pp_Runner.API.ExceptionFilters;
using Firefly_iii_pp_Runner.API.Exceptions;
using Firefly_iii_pp_Runner.API.Models.ThunderClient.Dtos;
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
    }
}
