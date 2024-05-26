﻿using Haondt.Web.Authentication.Pages;
using Haondt.Web.Pages;
using Haondt.Web.Persistence;
using Haondt.Web.Services;
using System.Diagnostics.CodeAnalysis;

namespace Haondt.Web.Authentication.Services
{
    public class SessionService : ISessionService
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationService _authenticationService;
        private Lazy<Task<UserSession?>> _userSessionLazy;
        public async Task<bool> IsAuthenticatedAsync() => await _userSessionLazy.Value != null;
        public async Task<StorageKey<User>> GetUserKeyAsync() => (await _userSessionLazy.Value)?.Owner ?? throw new InvalidOperationException(nameof(GetUserKeyAsync));

        private Lazy<string?> _sessionTokenLazy;
        public string? SessionToken => _forcedSessionToken ?? _sessionTokenLazy.Value;
        private string? _forcedSessionToken;

        [MemberNotNull(nameof(_sessionTokenLazy))]
        [MemberNotNull(nameof(_userSessionLazy))]
        public void Reset(string? sessionToken = null)
        {
            _forcedSessionToken = sessionToken;
            _sessionTokenLazy = new(() =>
            {
                if (!_authenticationService.TryGetAuthentication(out var sessionToken))
                    return null;
                if (string.IsNullOrEmpty(sessionToken))
                    return null;
                return sessionToken;
            });

            _userSessionLazy = new(async () =>
            {
                if (string.IsNullOrEmpty(SessionToken))
                    return null;

                var (success, session) = await _userService.TryGetSession(SessionToken);
                if (!success)
                    return null;
                return session;
            });

        }

        public SessionService(UserService userService, IHttpContextAccessor httpContextAccessor, AuthenticationService authenticationService)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _authenticationService = authenticationService;
            Reset();
        }
    }
}
