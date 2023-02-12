using FireflyIIIpp.Core.Models;
using FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.FireflyIII.Abstractions
{
    public interface IFireflyIIIService
    {

        public string PrepareQuery(List<RunnerQueryOperation> queryOperators);
        public Task<TransactionDto> GetTransaction(string id);
        public Task<ManyTransactionsContainerDto> GetTransactions(DateTime start, DateTime end, int page);
        public Task<ManyTransactionsContainerDto> GetTransactions(List<RunnerQueryOperation> queryOperators, int page);
        public Task UpdateTransaction(string transactionId, TransactionUpdateDto transaction, CancellationToken cancellationToken);
    }
}
