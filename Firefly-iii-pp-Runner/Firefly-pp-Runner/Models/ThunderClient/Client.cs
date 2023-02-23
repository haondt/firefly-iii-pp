using Firefly_iii_pp_Runner.Models.ThunderClient.Enums;
using Newtonsoft.Json;

namespace Firefly_iii_pp_Runner.Models.ThunderClient
{
    public class Client
    {
        [JsonProperty("_id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ColId { get; set; }
        public string ContainerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public double SortNum { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Modified { get; set; } = DateTime.Now;
        public List<object> Headers { get; set; } = new List<object>();
        public List<object> Params { get; set; } = new List<object>();
        public Body Body { get; set; }
        public List<Test> Tests { get; set; } = new List<Test>();
    }

    public class Body
    {
        public ClientBodyTypeEnum Type { get; set; }
        public string Raw { get; set; }
        public List<object> Form { get; set; } = new List<object>();
        public string Binary { get; set; }
    }
}
