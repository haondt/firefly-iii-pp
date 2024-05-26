﻿using FireflyIIIpp.Core.Exceptions;
using FireflyIIIpp.Core.Extensions;
using FireflyIIIpp.Core.Models;
using FireflyIIIpp.FireflyIII.Abstractions;
using FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos;
using FireflyIIIpp.FireflyIII.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;

namespace FireflyIIIpp.FireflyIII.Services
{
    public class FireflyIIIService : IFireflyIIIService
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
                { "start", start.ToString("yyyy-MM-dd") },
                { "end", end.ToString("yyyy-MM-dd") }
            });


            var result = await _httpClient.GetAsync(method);
            result.EnsureSuccessStatusCode();

            return await result.Content.ReadFromJsonAsync<ManyTransactionsContainerDto>() ?? throw new JsonSerializationException();
        }

        private string StringifyOperatorValue(object value)
        {
            if (value == null)
                throw new ArgumentException($"Cannot encode null query value");
            switch(value)
            {
                case string valueString:
                    if (valueString.Contains(' '))
                        return $"\"{valueString}\"";
                    return valueString;
                case bool valueBool:
                    return valueBool ? "true" : "false";
                case decimal:
                case double:
                case float:
                case int:
                case uint:
                case nint:
                case long:
                case ulong:
                case short:
                case ushort:
                    return value.ToString() ?? "";
                case DateTime valueDateTime:
                    return valueDateTime.ToString("yyyy-MM-dd");
                default:
                    throw new ArgumentException($"Unsure how to encode query value ${value} of type ${value.GetType().ToString()}");
            }
        }

        public string PrepareQuery(List<RunnerQueryOperation> queryOperators)
        {
            var groupedOperators = queryOperators.Select(o => $"{o.Operand}+{o.Operator}").GroupBy(v => v).Where(grp => grp.Count() > 1).Select(grp => grp.Key).ToList();
            if (groupedOperators.Count > 0)
                throw new ArgumentException($"Found multiple entries for the following operand+operator pairs: {string.Join(", ", groupedOperators)}");

            var query = string.Join(' ', queryOperators.Select(o =>
                $"{o.Operand}_{o.Operator}:{StringifyOperatorValue(o.Result)}"));
             query = HttpUtility.UrlEncode(query);
            return query;
        }

        public async Task<ManyTransactionsContainerDto> GetTransactions(List<RunnerQueryOperation> queryOperators, int page)
        {
            var query = PrepareQuery(queryOperators);
            var method = $"/api/v1/search/transactions?page={page}&query={query}";

            var result = await _httpClient.GetAsync(method);
            await result.EnsureDownstreamSuccessStatusCode("Firefly-iii");

            return await result.Content.ReadFromJsonAsync<ManyTransactionsContainerDto>() ?? throw new JsonSerializationException();
        }

        public async Task<TransactionDto> GetTransaction(string id)
        {
            var result = await _httpClient.GetAsync($"/api/v1/transactions/{id}");
            if (result.StatusCode == HttpStatusCode.NotFound)
                throw new NotFoundException(id);
            
            else if (result.StatusCode != HttpStatusCode.OK)
                throw new BadHttpRequestException(result.ReasonPhrase ?? "");

            var container = await result.Content.ReadFromJsonAsync<SingleTransactionContainerDto>() ?? throw new JsonSerializationException();
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
            await response.EnsureDownstreamSuccessStatusCode("Firefly-iii");
        }

        public async Task DeleteTransaction(string id)
        {
            var response = await _httpClient.DeleteAsync($"/api/v1/transactions/{id}");
            await response.EnsureDownstreamSuccessStatusCode("Firefly-iii");
        }

        public async Task<TransactionDto> CreateTransaction(CreateTransactionDto transaction)
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/v1/transactions", transaction);
            await response.EnsureDownstreamSuccessStatusCode("Firefly-iii");
            var container = await response.Content.ReadFromJsonAsync<SingleTransactionContainerDto>() ?? throw new JsonSerializationException();
            return container.Data;
        }
    }
}
