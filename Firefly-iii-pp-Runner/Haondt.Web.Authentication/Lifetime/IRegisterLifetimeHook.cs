using Haondt.Web.Authentication.Services;
using Haondt.Web.Liftetime;
using Haondt.Web.Persistence;

namespace Haondt.Web.Authentication.Lifetime
{
    public interface IRegisterLifetimeHook : ILifetimeHook
    {
        public Task OnRegisterAsync(User user, StorageKey<User> userKey);
    }
}
