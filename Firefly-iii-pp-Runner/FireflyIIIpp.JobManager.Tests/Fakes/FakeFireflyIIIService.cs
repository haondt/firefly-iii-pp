using FireflyIIIpp.Core.Models;
using FireflyIIIpp.FireflyIII.Abstractions;
using FireflyIIIpp.FireflyIII.Abstractions.Models;
using FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireflyIIIpp.Tests.Fakes
{
    public class FakeFireflyIIIService : IFireflyIIIService
    {

        public Dictionary<string, TransactionDto> Transactions { get; set; } = new Dictionary<string, TransactionDto>();
        public HashSet<string> UpdatedTransactions { get; set; } = new HashSet<string>();
        public int PageSize { get; set; } = 50;

        public Func<TransactionDto, bool> Query { get; set; } = t => true;

        private IEnumerable<T> GetPage<T>(IEnumerable<T> items, int page)
        {
            return items.Skip((page - 1) * PageSize).Take(PageSize);
        }

        public Task<TransactionDto> GetTransaction(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ManyTransactionsContainerDto> GetTransactions(DateTime start, DateTime end, int page)
        {
            throw new NotImplementedException();
        }

        public Task<ManyTransactionsContainerDto> GetTransactions(List<RunnerQueryOperation> queryOperators, int page)
        {
            var transactions = Transactions
                .Select(kvp => kvp.Value)
                .Where(Query).ToList();
            var pages = transactions.Count == 0 ? 1
                : (int)((transactions.Count - 1) / PageSize + 1);
            if (page < 1 || page > pages)
                throw new ArgumentException(nameof(pages));
            var pagedData = GetPage(transactions, page).ToList();
            return Task.FromResult(new ManyTransactionsContainerDto
            {
                Data = pagedData,
                Meta = new TransactionListMetadata
                {
                    Pagination = new PaginationData
                    {
                        Total = transactions.Count,
                        Count = pagedData.Count,
                        Per_page = PageSize,
                        Current_page = page,
                        Total_pages = pages
                    }
                }
            });
        }

        public string PrepareQuery(List<RunnerQueryOperation> queryOperators)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTransaction(string transactionId, TransactionUpdateDto transaction, CancellationToken cancellationToken)
        {
            Transactions[transactionId].Attributes.Transactions = transaction.Transactions;
            UpdatedTransactions.Add(transactionId);
            return Task.CompletedTask;
        }
    }
}
