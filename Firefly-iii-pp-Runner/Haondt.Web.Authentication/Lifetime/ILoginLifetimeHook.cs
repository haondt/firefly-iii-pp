using Haondt.Web.Authentication.Services;
using Haondt.Web.Liftetime;
using Haondt.Web.Persistence;

namespace Haondt.Web.Authentication.Lifetime
{
    public interface ILoginLifetimeHook : ILifetimeHook
    {
        public Task OnLoginAsync(string username, string password, StorageKey<User> userKey, string sessionToken);
    }
}
