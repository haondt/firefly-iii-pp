
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Firefly_iii_pp_Runner.API.Models.ThunderClient.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ClientBodyTypeEnum
    {
        Binary,
        Json
    }
}
