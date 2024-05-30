using Haondt.Web.Assets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Haondt.Web.Controllers
{
    [Route("favicon.ico")]
    [ApiController]
    public class FaviconController(IAssetProvider assetProvider) : Controller
    {
        public async Task<IActionResult> Get()
        {
            if (await assetProvider.GetAssetAsync("favicon.ico") is not { IsSuccessful: true, Value: var asset })
                return NotFound();
            return File(asset, "image/x-icon");
        }
    }
}
