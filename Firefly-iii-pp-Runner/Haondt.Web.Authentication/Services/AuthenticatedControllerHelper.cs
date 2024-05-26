using Haondt.Web.Controllers;
using Haondt.Web.Extensions;
using Haondt.Web.DynamicForm;
using Haondt.Web.Pages;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haondt.Web.Authentication.Pages;

namespace Haondt.Web.Authentication.Services
{
    public class AuthenticatedControllerHelper(
        IPageRegistry pageRegistry,
        ISessionService sessionService,
        IOptions<IndexSettings> options,
        UserService userService
        ) : ControllerHelper(pageRegistry, options), IAuthenticatedControllerHelper
    {
        private readonly IndexSettings _indexSettings = options.Value;
        private readonly IPageRegistry _pageRegistry = pageRegistry;

        public async Task<IActionResult> GetForceLoginView(BaseController controller)
        {
            var content = await _pageRegistry.GetPageFactory("dynamicForm").Create(new AlertDynamicFormFactory(

                "access denied",
                "please log in to continue",
                "partials/login"
                ).Create());

            return await GetModal(controller, content, false);
        }

        public async Task<(bool IsValid, IActionResult? InvalidSessionResponse)> VerifySession(BaseController controller)
        {
            if (!await sessionService.IsAuthenticatedAsync())
            {
                if (!string.IsNullOrEmpty(sessionService.SessionToken))
                    await userService.EndSession(sessionService.SessionToken);
                return (false, await GetForceLoginView(controller));
            }
            return (true, null);
        }
        public override async Task<IActionResult> GetView(BaseController controller, string page, Func<Task<IPageModel>>? modelFactory = null, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null)
        {
            if (!_pageRegistry.TryGetPageFactory(page, out var pageEntryFactory))
            {
                pageEntryFactory = _pageRegistry.GetPageFactory(_indexSettings.HomePage);
                modelFactory = null;
                responseOptions = null;
            }

            if (pageEntryFactory is INeedsAuthenticationPageEntryFactory)
                if (!await sessionService.IsAuthenticatedAsync())
                    return await GetForceLoginView(controller);

            var pageEntry = modelFactory != null
                ? await pageEntryFactory.Create(await modelFactory(), responseOptions)
                : await pageEntryFactory.Create(controller.Request.AsRequestData(), responseOptions);

            return pageEntry.CreateView(controller);
        }
    }
}
