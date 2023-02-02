using Firefly_iii_pp_Runner.API.Models.Postman.Enums;
using Newtonsoft.Json;

namespace Firefly_iii_pp_Runner.API.Models.Postman
{
    public class Request : Item
    {
        [JsonProperty("request")]
        public RequestProperties RequestInner { get; set; } = new RequestProperties();

        public List<object> Response { get; set; } = new List<object>();
    }

    public class RequestProperties
    {
        public string Method { get; set; }
        public List<object> Header { get; set; } = new List<object>();
        public Body Body { get; set; }
        public Url Url { get; set; }
    }

    public class Url
    {
        public string Raw { get; set; }
        public List<string> Host { get; set; } = new List<string>();
        public List<string> Path { get; set; } = new List<string>();
    }

    public class Body
    {
        public RequestBodyModeEnum Mode { get; set; }
        public string Raw { get; set; }
        public BodyOptions Options { get; set; }
    }

    public class BodyOptions
    {
        public BodyOption Raw { get; set; }
    }

    public class BodyOption
    {
        public RequestBodyOptionsLanguageEnum Language { get; set; }
    }
}
