using Firefly_iii_pp_Runner.API.ExceptionFilters;
using Firefly_iii_pp_Runner.API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Firefly_iii_pp_Runner.API.Controllers
{
    [ApiController]
    [JsonExceptionFilter(typeof(ArgumentException), 400)]
    [JsonExceptionFilter(typeof(JsonSerializationException), 400)]
    [JsonExceptionFilter(typeof(NotFoundException), 404, "The requested resource was not found")]
    [JsonExceptionFilter(typeof(RunnerBusyException), 503, "A job is currently running")]
    [InheritableExceptionFilter(typeof(Exception), 500)]
    [Produces("application/json")]
    public class BaseController : ControllerBase
    {
    }
}
