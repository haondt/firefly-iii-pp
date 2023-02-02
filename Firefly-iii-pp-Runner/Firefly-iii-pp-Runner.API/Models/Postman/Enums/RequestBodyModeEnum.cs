using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace Firefly_iii_pp_Runner.API.Models.Postman.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RequestBodyModeEnum
    {
        Raw
    }
}
