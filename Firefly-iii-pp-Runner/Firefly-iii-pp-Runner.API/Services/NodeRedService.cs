using Firefly_iii_pp_Runner.API.Models.FireflyIII;
using Firefly_iii_pp_Runner.API.Settings;
using Microsoft.Extensions.Options;

namespace Firefly_iii_pp_Runner.API.Services
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
