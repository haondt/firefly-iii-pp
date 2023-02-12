using FireflyIIIpp.NodeRed.Abstractions;

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
    }
}
