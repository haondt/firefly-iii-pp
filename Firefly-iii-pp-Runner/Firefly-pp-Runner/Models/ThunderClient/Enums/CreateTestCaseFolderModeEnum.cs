using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Firefly_iii_pp_Runner.Models.ThunderClient.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CreateTestCaseFolderModeEnum
    {
        [EnumMember(Value = "use-existing-or-create")]
        UseExistingOrCreate,
        [EnumMember(Value = "force-create-new")]
        ForceCreateNew
    }
}
