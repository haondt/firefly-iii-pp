using DotNext;
using DotNext.Collections.Generic;
using Haondt.Web.Assets;
using Haondt.Web.Extensions;
using Haondt.Web.Styles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace Haondt.Web.Controllers
{
    [Route("assets")]
    public class AssetsController(
        IAssetProvider assetProvider,
        IStylesProvider stylesProvider,
        FileExtensionContentTypeProvider contentTypeProvider) : BaseController
    {

        private static readonly Dictionary<string, string> _customContentTypes = new()
        {
            { "._hs", "text/hyperscript" }
        };

        [Route("{**assetPath}")]
        public async Task<IActionResult> Get(string assetPath)
        {
            if ("style.css".Equals(assetPath))
                return  Content(await stylesProvider.GetStylesAsync(), "text/css");

            var contentTypeResult = _customContentTypes.TryGetValue(Path.GetExtension(assetPath))
                | contentTypeProvider.TryGetContentType(assetPath);

            if (!contentTypeResult.HasValue)
                return BadRequest("Unsupported file type");

            if (await assetProvider.GetAssetAsync(assetPath) is not { IsSuccessful: true, Value: var asset })
                return NotFound();

            return File(asset, contentTypeResult.Value);
        }

    }
}
