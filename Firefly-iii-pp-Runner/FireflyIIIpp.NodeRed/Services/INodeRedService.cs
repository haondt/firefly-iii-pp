using FireflyIIIpp.NodeRed.Reasons;
using Haondt.Core.Models;

namespace FireflyIIIpp.NodeRed.Services
{
    public interface INodeRedService
    {
        public Task<Result<Result<string, ApplyRulesReason>, string>> ApplyRules(string input, CancellationToken? cancellationToken = null);
        public Task<Result<string, string>> ExtractKey(string field, string input, CancellationToken? cancellationToken = null);
    }
}