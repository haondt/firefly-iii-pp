using Haondt.Web.Persistence;

namespace Haondt.Web.Authentication.Services
{
    public class UserSession
    {
        public static StorageKey<UserSession> GetStorageKey(string sessionToken) => StorageKey<UserSession>.Create(sessionToken);
        public required DateTime Expiry { get; set; }
        public required StorageKey<User> Owner { get; set; }
    }
}
