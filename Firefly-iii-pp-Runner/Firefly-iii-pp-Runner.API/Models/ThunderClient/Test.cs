using Firefly_iii_pp_Runner.API.Models.ThunderClient.Enums;

namespace Firefly_iii_pp_Runner.API.Models.ThunderClient
{
    public class Test
    {
        public TestTypeEnum Type { get; set; }
        public string Value { get; set; } = string.Empty;
        public TestActionEnum Action { get; set; }
        public string Custom { get; set; } = string.Empty;
    }
}
