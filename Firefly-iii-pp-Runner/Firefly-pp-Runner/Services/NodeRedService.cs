using Firefly_iii_pp_Runner.Models.FireflyIII;
using Firefly_iii_pp_Runner.Settings;
using Firefly_pp_Runner.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;

namespace Firefly_iii_pp_Runner.Services
{
    public class NodeRedService
    {
        private readonly NodeRedSettings _settings;
        private readonly ILogger<NodeRedService> _logger;
        private readonly HttpClient _httpClient;

        public NodeRedService(IOptions<NodeRedSettings> options, ILogger<NodeRedService> logger, IHttpClientFactory httpClientFactory)
        {
            _settings = options.Value;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<TransactionPartDto> ApplyRules(TransactionPartDto transactionPart, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/apply", transactionPart, cancellationToken);
            await response.EnsureDownstreamSuccessStatusCode("Node-Red");

            return await response.Content.ReadFromJsonAsync<TransactionPartDto>();
        }

        public async Task<string> ApplyRules(string input)
        {
            var content = new StringContent(input, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/apply", content);
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
