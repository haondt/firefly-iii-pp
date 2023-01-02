using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Firefly_iii_pp_Runner.API.Models
{
    public class RunnerStatus
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RunnerState State { get; set; } = RunnerState.Completed;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int CompletedTransactions { get; set; }
        public int TotalTransactions { get; set; }
    }

    public enum RunnerState
    {
        Running,
        Failed,
        Stopped,
        Completed
    }
}
