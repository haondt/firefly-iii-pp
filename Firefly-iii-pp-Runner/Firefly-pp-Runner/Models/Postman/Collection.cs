
using Newtonsoft.Json;

namespace Firefly_iii_pp_Runner.Models.Postman
{
    public class Collection
    {
        public CollectionInfo Info { get; set; }
        public List<Item> Item { get; set; } = new List<Item>();
        public List<Event> Event { get; set; } = new List<Event>();
        public List<Variable> Variable { get; set; } = new List<Variable>();
    }

    public class CollectionInfo
    {
        [JsonProperty("_postman_id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public string UpdatedAt { get; set; }
    }
}
