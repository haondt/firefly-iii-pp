using FireflyIIIpp.Core.Extensions;
using FireflyIIIpp.NodeRed.Abstractions;
using FireflyIIIpp.NodeRed.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;

namespace FireflyIIIpp.NodeRed.Services
{
    public class NodeRedService : INodeRedService
    {
        private readonly NodeRedSettings _settings;
        private readonly ILogger<NodeRedService> _logger;
        private readonly HttpClient _httpClient;

        public NodeRedService(IOptions<NodeRedSettings> options, ILogger<NodeRedService> logger, HttpClient httpClient)
        {
            _settings = options.Value;
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<string> ApplyRules(string input, CancellationToken? cancellationToken = null)
        {
            var content = new StringContent(input, Encoding.UTF8, "application/json");
            var response = cancellationToken.HasValue
                ? await _httpClient.PostAsync("/apply", content, cancellationToken.Value)
                : await _httpClient.PostAsync("/apply", content);
            await response.EnsureDownstreamSuccessStatusCode("Node-Red");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task ExportFlows()
        {
            var response = await _httpClient.PostAsync("/export-flows", null);
            await response.EnsureDownstreamSuccessStatusCode("Node-Red");
        }
    }
}
