using Firefly_iii_pp_Runner.API.Models.FireflyIII;
using Firefly_iii_pp_Runner.API.Settings;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace Firefly_iii_pp_Runner.API.Services
{
    public class FireflyIIIService
    {
        private readonly FireflyIIISettings _settings;
        private readonly ILogger<FireflyIIIService> _logger;
        private readonly HttpClient _httpClient;

        public FireflyIIIService(IOptions<FireflyIIISettings> options, ILogger<FireflyIIIService> logger, HttpClient httpClient)
        {
            _settings = options.Value;
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Token);
        }

        public async Task<ManyTransactionsContainerDto> GetTransactions(DateTime start, DateTime end, int page)
        {
            var method = QueryHelpers.AddQueryString("/api/v1/transactions", new Dictionary<string, string?>
            {
                { "page", page.ToString() },
                { "start", start.ToString("yyyy-M-d") },
                { "end", end.ToString("yyyy-M-d") }
            });


            var result = await _httpClient.GetAsync(method);
            result.EnsureSuccessStatusCode();

            return await result.Content.ReadFromJsonAsync<ManyTransactionsContainerDto>();
        }

        public async Task<TransactionDto> GetTransaction(string id)
        {
            var result = await _httpClient.GetAsync($"/api/v1/transactions/{id}");
            result.EnsureSuccessStatusCode();

            var container = await result.Content.ReadFromJsonAsync<SingleTransactionContainerDto>();
            return container.Data;
        }

        public async Task UpdateTransaction(string transactionId, TransactionUpdateDto transaction, CancellationToken cancellationToken)
        {
                var response = await _httpClient.PutAsJsonAsync($"/api/v1/transactions/{transactionId}", transaction, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Got non-200 response when updating transaction" +
                        $"\nstatus: {response.StatusCode}" +
                        $"\nrequest: {await response.RequestMessage.Content.ReadFromJsonAsync<object>()}" +
                        $"\nresponse: {await response.Content.ReadFromJsonAsync<object>()}");
                }
                response.EnsureSuccessStatusCode();
        }
    }
}
