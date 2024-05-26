using Haondt.Web.Persistence;

namespace Haondt.Web.Authentication.Services
{
    public interface ISessionService
    {
        public Task<bool> IsAuthenticatedAsync();
        public Task<StorageKey<User>> GetUserKeyAsync();
        public void Reset(string? sessionToken = null);
        public string? SessionToken { get; }
    }
}
