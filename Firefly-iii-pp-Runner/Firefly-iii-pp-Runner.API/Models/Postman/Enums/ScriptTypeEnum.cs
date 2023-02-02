using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Firefly_iii_pp_Runner.API.Models.Postman.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ScriptTypeEnum
    {
        [EnumMember(Value = "text/javascript")]
        Javascript
    }
}
