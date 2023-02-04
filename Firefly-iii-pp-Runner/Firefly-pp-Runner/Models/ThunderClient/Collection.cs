using Newtonsoft.Json;

namespace Firefly_iii_pp_Runner.Models.ThunderClient
{
    public class Collection
    {
        [JsonProperty("_id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ColName { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public int SortNum { get; set; }
        public List<Folder> Folders { get; set; } = new List<Folder>();
        public Settings Settings { get; set; } = new Settings();
    }
}
