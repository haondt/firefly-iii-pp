using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Haondt.Web.Services;

namespace Haondt.Web.Controllers
{
    [Route("partials")]
    public class PartialsController(IControllerHelper helper) : BaseController
    {
        [Route("{page}")]
        public Task<IActionResult> GetPartialView([FromRoute] string page)
        {
            return helper.GetView(this, page);
        }
    }
}
