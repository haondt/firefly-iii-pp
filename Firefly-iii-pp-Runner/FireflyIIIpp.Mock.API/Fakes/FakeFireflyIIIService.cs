using Bogus;
using FireflyIIIpp.Core.Extensions;
using FireflyIIIpp.FireflyIII.Abstractions;
using FireflyIIIpp.FireflyIII.Abstractions.Models;
using FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos;
using FireflyIIIpp.Mock.API.Settings;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Web;

namespace FireflyIIIpp.Mock.API.Fakes
{
    public class FakeFireflyIIIService : IFireflyIIIService
    {
        private readonly FakeFireflyIIIServiceSettings _settings;
        private readonly Faker _faker;
        private int _lastUnusedId = 1;
        private readonly Dictionary<string, TransactionDto> _transactions = new Dictionary<string, TransactionDto>();

        public FakeFireflyIIIService(IOptions<FakeFireflyIIIServiceSettings> options)
        {
            _settings = options.Value;
            _faker = new Faker();
            for(int i=0; i<200;i++)
            {
                var t = GenerateTransactionData();
                _transactions[t.Id] = t;
            }
        }

        private TransactionDto GenerateTransactionData()
        {
            var type = _faker.PickRandom("withdrawal", "deposit");
            var partDto = new Faker<TransactionPartDto>()
                .RuleFor(t => t.Type, f => type)
                .RuleFor(t => t.Date, f => f.Date.Recent().ToString("o", CultureInfo.InvariantCulture))
                .RuleFor(t => t.Amount, f => f.Finance.Amount().ToString())
                .RuleFor(t => t.Description, f => f.Commerce.ProductName())
                .RuleFor(t => t.Source_name, f => type == "withdrawal" ? f.Finance.AccountName() : f.Company.CompanyName())
                .RuleFor(t => t.Destination_name, f => type == "deposit" ? f.Finance.AccountName() : f.Company.CompanyName())
                .Generate();

            var dto = new TransactionDto
            {
                Id = (_lastUnusedId++).ToString(),
                Attributes = new TransactionAttributes
                {
                    Created_At = _faker.Date.Past(1, DateTime.Parse(partDto.Date)),
                    Updated_At = _faker.Date.Past(1, DateTime.Parse(partDto.Date)),
                    Transactions = new List<TransactionPartDto> { partDto }
                }
            };
            return dto;
        }

        public async Task<TransactionDto> GetTransaction(string id)
        {
            await Task.Delay(_settings.HttpDelayInMilliseconds);
            return _transactions[id];
        }

        public Task<ManyTransactionsContainerDto> GetTransactions(DateTime start, DateTime end, int page)
        {
            throw new NotImplementedException();
        }

        public async Task<ManyTransactionsContainerDto> GetTransactions(List<FireflyIIIpp.Core.Models.RunnerQueryOperation> queryOperators, int page)
        {
            await Task.Delay(_settings.HttpDelayInMilliseconds);
            var transactions = _transactions.OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value)
                .Page(_settings.PageSize, page).ToList();

            return new ManyTransactionsContainerDto
            {
                Data = transactions,
                Meta = new TransactionListMetadata
                {
                    Pagination = new PaginationData
                    {
                        Count = transactions.Count,
                        Current_page = page,
                        Per_page = _settings.PageSize,
                        Total = _transactions.Count,
                        Total_pages = _transactions.Pages(_settings.PageSize)
                    }
                }
            };
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
                    return value.ToString();
                case DateTime valueDateTime:
                    return valueDateTime.ToString("yyyy-MM-dd");
                default:
                    throw new ArgumentException($"Unsure how to encode query value ${value} of type ${value.GetType().ToString()}");
            }
        }

        public string PrepareQuery(List<FireflyIIIpp.Core.Models.RunnerQueryOperation> queryOperators)
        {
            var groupedOperators = queryOperators.Select(o => $"{o.Operand}+{o.Operator}").GroupBy(v => v).Where(grp => grp.Count() > 1).Select(grp => grp.Key).ToList();
            if (groupedOperators.Count > 0)
                throw new ArgumentException($"Found multiple entries for the following operand+operator pairs: {string.Join(", ", groupedOperators)}");

            var query = string.Join(' ', queryOperators.Select(o =>
                $"{o.Operand}_{o.Operator}:{StringifyOperatorValue(o.Result)}"));
             query = HttpUtility.UrlEncode(query);
            return query;
        }

        public async Task UpdateTransaction(string transactionId, TransactionUpdateDto transaction, CancellationToken cancellationToken)
        {
            await Task.Delay(_settings.HttpDelayInMilliseconds, cancellationToken);
            _transactions[transactionId].Attributes.Transactions = transaction.Transactions;
        }

        public Task DeleteTransaction(string id)
        {
            _transactions.Remove(id);
            return Task.CompletedTask;
        }

        public Task<TransactionDto> CreateTransaction(CreateTransactionDto transaction)
        {
            var t = GenerateTransactionData();
            _transactions[t.Id] = t;
            return Task.FromResult(t);
        }
    }
}
