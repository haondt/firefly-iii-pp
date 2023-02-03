using Firefly_iii_pp_Runner.API.Models.ThunderClient.Enums;

namespace Firefly_iii_pp_Runner.API.Models.ThunderClient.Dtos
{
    public class CreateTestCaseRequestDto
    {
        public Dictionary<string, string> BodyFields { get; set; } = new Dictionary<string, string>();
        public string FolderName { get; set; }
        public CreateTestCaseFolderModeEnum CreateFolderMode { get; set; }
        public bool ConfigureExpectedValues { get; set; }
        public Dictionary<string, string> ExpectedValues { get; set; } = new Dictionary<string, string>();
    }
}
