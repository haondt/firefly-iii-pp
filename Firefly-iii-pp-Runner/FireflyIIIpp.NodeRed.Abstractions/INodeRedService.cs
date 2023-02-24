using FireflyIIIpp.NodeRed.Abstractions.Models.Dtos;

namespace FireflyIIIpp.NodeRed.Abstractions
{
    public interface INodeRedService
    {
        public Task<(bool hasChanges, string output)> TryApplyRules(string input, CancellationToken? cancellationToken = null);
        public Task<NodeRedExtractKeyResponseDto> ExtractKey(string field, string input, CancellationToken? cancellationToken = null);

        public Task ExportFlows();

    }
}