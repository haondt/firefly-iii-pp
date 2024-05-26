namespace Haondt.Web.Persistence
{
    public class PersistenceSettings
    {
        public bool UseReadCaching { get; set; } = true;
        public TimeSpan CacheLifetime { get; set; } = TimeSpan.FromHours(1);
    }
}
