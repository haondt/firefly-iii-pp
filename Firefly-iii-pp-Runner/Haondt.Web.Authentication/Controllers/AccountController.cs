﻿using Haondt.Web.Authentication.Lifetime;
using Haondt.Web.Authentication.Pages;
using Haondt.Web.Authentication.Services;
using Haondt.Web.Controllers;
using Haondt.Web.Liftetime;
using Haondt.Web.Pages;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Haondt.Web.Authentication.Controllers
{
    [Route("account")]
    public class AccountController(
        IControllerHelper helper,
        UserService userService,
        IPageRegistry pageRegistry,
        IOptions<IndexSettings> indexOptions,
        ISessionService sessionService,
        LifetimeHookService lifetimeHookService,
        AuthenticationService authenticationService) : BaseController
    {
        private readonly IPageRegistry _pageRegistry = pageRegistry;
        private readonly IndexSettings _indexSettings = indexOptions.Value;
        private readonly ISessionService _sessionService = sessionService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string? username, [FromForm] string? password)
        {
            username ??= "";
            password ??= "";
            var (result, sessionToken, sessionExpiry ,userKey) = await userService.TryAuthenticateUserAndGenerateSessionToken(username, password);
            if (!result)
                return await helper.GetView(this, "login", new LoginDynamicFormFactory(username, "incorrect username or password"));

            _sessionService.Reset(sessionToken);
            await lifetimeHookService.FireAsync<ILoginLifetimeHook>(h => h.OnLoginAsync(username, password, userKey, sessionToken));
            authenticationService.AddAuthentication(sessionToken, sessionExpiry);
            return await helper.GetView(this, _indexSettings.HomePage);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            if (await _sessionService.IsAuthenticatedAsync())
            {
                var sessionToken = _sessionService.SessionToken;
                await userService.EndSession(sessionToken!);
                authenticationService.ExpireAuthentication();
            }

            return await helper.GetView(this, _indexSettings.AuthenticationPage);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] string? username, [FromForm] string? password)
        {
            username ??= "";
            password ??= "";
            var (result, usernameReason, passwordReason, user, userKey) = await userService.TryRegisterUser(username, password);
            if (!result)
                return await helper.GetView(this, "register", new RegisterDynamicFormFactory(username, usernameReason, passwordReason));

            await lifetimeHookService.FireAsync<IRegisterLifetimeHook>(h => h.OnRegisterAsync(user!, userKey));
            return await helper.GetView(this, _indexSettings.AuthenticationPage);
        }
    }
}
