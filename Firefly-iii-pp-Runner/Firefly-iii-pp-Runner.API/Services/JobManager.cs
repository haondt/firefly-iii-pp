using Firefly_iii_pp_Runner.API.Exceptions;
using Firefly_iii_pp_Runner.API.Models;
using Firefly_iii_pp_Runner.API.Models.FireflyIII;
using Newtonsoft.Json;

namespace Firefly_iii_pp_Runner.API.Services
{
    public class JobManager
    {
        private readonly ILogger<JobManager> _logger;
        private readonly FireflyIIIService _fireflyIII;
        private readonly NodeRedService _nodeRed;
        private readonly RunnerStatus _status;
        private CancellationTokenSource _tokenSource;

        public JobManager(ILogger<JobManager> logger, FireflyIIIService fireflyIII, NodeRedService nodeRed)
        {
            _logger = logger;
            _fireflyIII = fireflyIII;
            _nodeRed = nodeRed;
            _status = new RunnerStatus();
        }

        public RunnerStatus GetStatus() => _status;

        public async Task<RunnerStatus> StartSingle(RunnerSingleDto dto)
        {
            if (_status.State == RunnerState.Running)
            {
                throw new RunnerBusyException();
            }

            if (_tokenSource != null)
                _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
            try
            {
                var initialRequest = await _fireflyIII.GetTransaction(dto.Id);

                if (initialRequest.Attributes.Transactions.Count != 1)
                    throw new InvalidOperationException("Multi-part transaction");

                _status.State = RunnerState.Running;
                _status.CompletedTransactions = 0;
                _status.TotalTransactions = 1;
                _status.CurrentPage = 1;
                _status.TotalPages = 1;
                _ = Task.Run(async () =>
                {

                    try
                    {
                        await UpdateTransaction(initialRequest, _tokenSource.Token);
                        _status.State = RunnerState.Completed;
                    }
                    catch (TaskCanceledException)
                    {

                    }
                    catch
                    {
                        _status.State = RunnerState.Failed;
                        throw;
                    }
                });
            }
            catch
            {
                _status.State = RunnerState.Failed;
                throw;
            }
            return _status;
        }

        public async Task<RunnerStatus> StartJob(RunnerDto dto)
        {
            if (_status.State == RunnerState.Running)
            {
                throw new RunnerBusyException();
            }

            if (_tokenSource != null)
                _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
            try
            {
                var initialRequest = await _fireflyIII.GetTransactions(dto.Start, dto.End, 1);
                _status.State = RunnerState.Running;
                _status.CompletedTransactions = 0;
                _status.TotalTransactions = initialRequest.Meta.Pagination.Total;
                _status.CurrentPage = 1;
                _status.TotalPages = initialRequest.Meta.Pagination.Total_pages;
                _ = Task.Run(() => JobTask(dto.Start, dto.End, initialRequest, _tokenSource.Token));
            }
            catch
            {
                _status.State = RunnerState.Failed;
                throw;
            }
            return _status;
        } 

        private async Task UpdateTransaction(TransactionDto transaction, CancellationToken cancellationToken)
        {
            if (transaction.Attributes.Transactions.Count != 1)
                return;
            if (transaction.Attributes.Transactions[0].Destination_name != "(no name)")
                return;

            var transactionData = transaction.Attributes.Transactions[0];
            var newTransactionData = await _nodeRed.ApplyRules(transactionData, cancellationToken);
            var updateDto = new TransactionUpdateDto
            {
                Apply_rules = false,
                Fire_webhooks = true,
                Transactions = new List<TransactionPartDto> { newTransactionData }
            };
            if (!newTransactionData.Equals(transactionData))
            {
                await _fireflyIII.UpdateTransaction(transaction.Id, updateDto, cancellationToken);
                _logger.LogInformation($"Updated transaction {transaction.Id} with dest {newTransactionData.Destination_name} (was: {transactionData.Destination_name})");
            }
        }

        private async Task JobTask(DateTime start, DateTime end, ManyTransactionsContainerDto initialRequest, CancellationToken cancellationToken)
        {
            try
            {
                var currentSet = initialRequest;
                while (true)
                {
                    await Task.WhenAll(currentSet.Data.Select(t => Task.Run(async () =>
                    {
                        await UpdateTransaction(t, cancellationToken);
                        _status.CompletedTransactions++;
                    })));

                    if (cancellationToken.IsCancellationRequested)
                        break;
                    else if (_status.CurrentPage < _status.TotalPages)
                        _status.CurrentPage++;
                    else
                        break;

                    currentSet = await _fireflyIII.GetTransactions(start, end, _status.CurrentPage);
                }
                _status.State = RunnerState.Completed;
            }
            catch (TaskCanceledException)
            {

            }
            catch
            {
                _status.State = RunnerState.Failed;
                throw;
            }
        }

        public RunnerStatus StopJob()
        {
            if (_status.State == RunnerState.Running)
            {
                _tokenSource.Cancel();
                _status.State = RunnerState.Stopped;
            }
            return _status;
        }
    }
}
