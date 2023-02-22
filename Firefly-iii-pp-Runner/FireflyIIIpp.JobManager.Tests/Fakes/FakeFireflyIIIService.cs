using FireflyIIIpp.Core.Extensions;
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
        public Queue<Func<TransactionDto, bool>> QueryQueue { get; set; } = new Queue<Func<TransactionDto, bool>>();

        public void AddQuery(Func<TransactionDto, bool> query)
        {
            QueryQueue.Enqueue(query);
        }

        public Task<TransactionDto> CreateTransaction(CreateTransactionDto transaction)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTransaction(string id)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionDto> GetTransaction(string id)
        {
            return Task.FromResult(Transactions[id]);
        }

        public Task<ManyTransactionsContainerDto> GetTransactions(DateTime start, DateTime end, int page)
        {
            throw new NotImplementedException();
        }

        public Task<ManyTransactionsContainerDto> GetTransactions(List<RunnerQueryOperation> queryOperators, int page)
        {
            var query = QueryQueue.Count > 0 ? QueryQueue.Dequeue() : Query;
            var transactions = Transactions
                .Select(kvp => kvp.Value)
                .Where(query).ToList();
            var pages = transactions.Count == 0 ? 1 : transactions.Pages(PageSize);
            if (page < 1 || page > pages)
                throw new ArgumentException(nameof(pages));
            var pagedData = transactions.Page(PageSize, page).ToList();
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
