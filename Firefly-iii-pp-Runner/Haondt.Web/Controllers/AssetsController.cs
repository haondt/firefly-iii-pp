﻿using Haondt.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace Haondt.Web.Controllers
{
    [Route("assets")]
    public class AssetsController(
        AssetProvider assetProvider,
        StylesProvider stylesProvider,
        FileExtensionContentTypeProvider contentTypeProvider) : BaseController
    {

        private Dictionary<string, string> _customContentTypes = new()
        {
            { "._hs", "text/hyperscript" }
        };

        [Route("{**assetPath}")]
        public IActionResult Get(string assetPath)
        {
            if ("style.css".Equals(assetPath))
                return  Content(stylesProvider.GetStyles(), "text/css");

            if (assetPath.Contains('/') || assetPath.Contains('\\'))
                return BadRequest("Invalid path.");;

            if (!_customContentTypes.TryGetValue(Path.GetExtension(assetPath), out var contentType))
                if (!contentTypeProvider.TryGetContentType(assetPath, out contentType))
                    return BadRequest("Unsupported file type.");

            if (!assetProvider.TryGetAsset(assetPath, out var content))
                return NotFound();

            return File(content, contentType);
        }
    }
}
