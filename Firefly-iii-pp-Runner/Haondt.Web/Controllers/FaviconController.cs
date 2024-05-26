using Haondt.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Haondt.Web.Controllers
{
    [Route("favicon.ico")]
    [ApiController]
    public class FaviconController(AssetProvider assetProvider) : Controller
    {
        public IActionResult Get()
        {
            if (!assetProvider.TryGetAsset("favicon.ico", out var content))
                return NotFound();
            return File(content, "image/x-icon");
        }
    }
}
