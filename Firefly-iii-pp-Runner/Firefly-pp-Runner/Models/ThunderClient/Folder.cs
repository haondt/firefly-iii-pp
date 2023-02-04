using Newtonsoft.Json;

namespace Firefly_iii_pp_Runner.Models.ThunderClient
{
    public class Folder
    {

        [JsonProperty("_id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string ContainerId { get; set; } = "";
        public DateTime Created { get; set; } = DateTime.Now;
        public int SortNum { get; set; }
        public Settings Settings { get; set; } = new Settings();
    }
}
