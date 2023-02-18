using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Firefly_pp_Runner.Models.Lookup.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LookupActionEnum
    {
        [EnumMember(Value = "none")]
        None,
        [EnumMember(Value = "get-keys")]
        GetKeys,
        [EnumMember(Value = "add-key")]
        AddKey,
        [EnumMember(Value = "delete-key")]
        DeleteKey,
        [EnumMember(Value = "get-key-value-value")]
        GetKeyValueValue,
        [EnumMember(Value = "get-value-value")]
        GetValueValue,
        [EnumMember(Value = "get-key-value")]
        GetKeyValue,
        [EnumMember(Value = "put-value")]
        PutValue,
        [EnumMember(Value = "delete-value")]
        DeleteValue,
        [EnumMember(Value = "autocomplete-value")]
        AutoCompleteValue
    }
}
