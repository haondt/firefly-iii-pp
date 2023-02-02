using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Firefly_iii_pp_Runner.API.Models.ThunderClient.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TestTypeEnum
    {
        [EnumMember(Value = "res-code")]
        ResCode,

        [EnumMember(Value = "json-query")]
        JsonQuery
    }
}
