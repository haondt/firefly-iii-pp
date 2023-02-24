using FireflyIIIpp.NodeRed.Abstractions;
using FireflyIIIpp.NodeRed.Abstractions.Models.Dtos;

namespace FireflyIIIpp.Mock.API.Fakes
{
    public class FakeNodeRedService : INodeRedService
    {
        public Task<(bool, string)> TryApplyRules(string input, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult((true, input));
        }

        public Task ExportFlows()
        {
            return Task.CompletedTask;
        }

        public Task<NodeRedExtractKeyResponseDto> ExtractKey(string field, string input, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult(new NodeRedExtractKeyResponseDto { Key = input });
        }
    }
}
