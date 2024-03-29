﻿using Firefly_iii_pp_Runner.ExceptionFilters;
using FireflyIIIpp.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly.Timeout;

namespace Firefly_iii_pp_Runner.Controllers
{
    [ApiController]
    [JsonExceptionFilter(typeof(ArgumentException), 400)]
    [JsonExceptionFilter(typeof(JsonSerializationException), 400)]
    [JsonExceptionFilter(typeof(NotFoundException), 404, "The requested resource was not found")]
    [JsonExceptionFilter(typeof(BusyException), 409, "A job is currently running")]
    [JsonExceptionFilter(typeof(NotReadyException), 409, "The requested resource is not yet ready")]
    [JsonExceptionFilter(typeof(ConflictException), 409)]
    [JsonExceptionFilter(typeof(DownstreamException), 502, "A downstream resource could not complete successfully")]
    [JsonExceptionFilter(typeof(TimeoutRejectedException), 502, "A downstream resource did not complete within the allotted timeout period")]
    [InheritableExceptionFilter(typeof(Exception), 500)]
    [Produces("application/json")]
    public class BaseController : ControllerBase
    {
    }
}
