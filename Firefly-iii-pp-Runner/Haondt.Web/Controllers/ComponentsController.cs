using Haondt.Web.Exceptions;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Haondt.Web.Controllers
{
    [Route("components")]
    public class ComponentsController(IControllerHelper helper) : BaseController
    {
        [HttpGet("toast")]
        public Task<IActionResult> GetToast([FromQuery] ToastSeverity severity, [FromQuery] string message) => helper.GetToastView(this, severity, message);
    }
}
