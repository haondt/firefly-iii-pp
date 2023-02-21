using FireflyIIIpp.NodeRed.Abstractions;
using FireflyIIIpp.NodeRed.Abstractions.Models.Dtos;

namespace FireflyIIIpp.Mock.API.Fakes
{
    public class FakeNodeRedService : INodeRedService
    {
        public Task<string> ApplyRules(string input, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult(input);
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
