using FireflyIIIpp.Components.Extensions;
using FireflyIIIpp.Components.Pages;
using Haondt.Web.Core.Controllers;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Firefly_iii_pp.Controllers
{
    public class SiteController(IPageComponentFactory pageFactory) : BaseController
    {
        [HttpGet("/")]
        public IActionResult Get()
        {
            return Redirect("node-red");
        }

        [HttpGet("{page}")]
        public async Task<IActionResult> Get(string page)
        {
            var enumeratedPage = PageComponentMap.PageFromPathName[page];
            var component = await PageComponentMap.PageComponentFactories[enumeratedPage](pageFactory);
            return component.CreateView(this);
        }
    }
}
