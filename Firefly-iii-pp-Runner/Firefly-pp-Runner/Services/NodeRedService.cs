using Firefly_iii_pp_Runner.Models.FireflyIII;
using Firefly_iii_pp_Runner.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

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
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TransactionPartDto>();
        }
    }
}
