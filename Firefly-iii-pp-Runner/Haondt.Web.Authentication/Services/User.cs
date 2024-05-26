using Haondt.Web.Persistence;

namespace Haondt.Web.Authentication.Services
{
    public class User
    {
        public static StorageKey<User> GetStorageKey(string username) => StorageKey<User>.Create(username.ToLower().Trim());
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string PasswordSalt { get; set; }
    }
}
