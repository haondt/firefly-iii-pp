namespace Haondt.Web.Services
{
    public class IndexSettings
    {
        public required string SiteName { get; set; }
        public required string HomePage { get; set; }
        public required string AuthenticationPage { get; set; }
        public List<string> NavigationBarPages { get; set; } = [];
    }
}
