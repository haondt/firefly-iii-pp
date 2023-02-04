using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Firefly_iii_pp_Runner.Models.Postman.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RequestBodyModeEnum
    {
        [EnumMember(Value = "raw")]
        Raw
    }
}
