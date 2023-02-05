using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Firefly_pp_Runner.Models.Runner
{
    public class RunnerStatus
    {
        public RunnerState State { get; set; } = RunnerState.Completed;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int CompletedTransactions { get; set; }
        public int TotalTransactions { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RunnerState
    {
        [EnumMember(Value = "running")]
        Running,
        [EnumMember(Value = "failed")]
        Failed,
        [EnumMember(Value = "stopped")]
        Stopped,
        [EnumMember(Value = "completed")]
        Completed
    }
}
