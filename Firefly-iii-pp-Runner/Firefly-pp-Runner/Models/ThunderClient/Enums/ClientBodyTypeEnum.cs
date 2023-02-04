
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Firefly_iii_pp_Runner.Models.ThunderClient.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ClientBodyTypeEnum
    {
        [EnumMember(Value = "binary")]
        Binary,
        [EnumMember(Value = "json")]
        Json
    }
}
