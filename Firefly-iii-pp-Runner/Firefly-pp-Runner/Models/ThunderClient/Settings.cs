namespace Firefly_iii_pp_Runner.Models.ThunderClient
{
    public class Settings
    {
        public List<object> Headers { get; set; } = new List<object>();
        public AuthSettings Auth { get; set; }
        public List<Test> Tests { get; set; } = new List<Test>();
        public OptionsSettings Options { get; set; }
        public Guid? EnvId { get; set; }
        public Dictionary<string, object> RunOptions { get; set; }
    }

    public class AuthSettings
    {
        public string Type { get; set; }
        public string Bearer { get; set; }
    }

    public class OptionsSettings
    {
        public string BaseUrl { get; set; }
    }
}
