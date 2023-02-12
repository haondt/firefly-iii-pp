namespace FireflyIIIpp.NodeRed.Abstractions
{
    public interface INodeRedService
    {
        public Task<string> ApplyRules(string input, CancellationToken? cancellationToken = null);

        public Task ExportFlows();

    }
}