using Firefly_iii_pp_Runner.Exceptions;
using Firefly_iii_pp_Runner.Models;
using Firefly_iii_pp_Runner.Models.FireflyIII;
using Firefly_pp_Runner.Models.Runner;
using Firefly_pp_Runner.Models.Runner.Dtos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Firefly_iii_pp_Runner.Services
{
    public class JobManager
    {
        private readonly ILogger<JobManager> _logger;
        private readonly FireflyIIIService _fireflyIII;
        private readonly NodeRedService _nodeRed;
        private readonly RunnerStatus _status;
        private int _completedTransactions = 0;
        private CancellationTokenSource _tokenSource;

        public JobManager(ILogger<JobManager> logger, FireflyIIIService fireflyIII, NodeRedService nodeRed)
        {
            _logger = logger;
            _fireflyIII = fireflyIII;
            _nodeRed = nodeRed;
            _status = new RunnerStatus();
        }

        public RunnerStatus GetStatus()
        {
            _status.CompletedTransactions = _completedTransactions;
            return _status;
        }

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
                _completedTransactions = 0;
                _status.TotalTransactions = 1;
                _status.CurrentPage = 1;
                _status.TotalPages = 1;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await UpdateTransaction(initialRequest, _tokenSource.Token);
                        _status.State = RunnerState.Completed;
                        _status.CompletedTransactions = 1;
                        _completedTransactions = 1;
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

        public Task<RunnerStatus> StartJob(RunnerDto dto)
        {
            var start = dto.Start.HasValue ? dto.Start.Value : throw new ArgumentNullException("start date");
            var end = dto.End.HasValue ? dto.End.Value : throw new ArgumentNullException("end date");
            return StartJob(p => _fireflyIII.GetTransactions(start, end, p));
        } 

        public Task<RunnerStatus> StartJob(QueryStartJobRequestDto dto)
        {
            return StartJob(p => _fireflyIII.GetTransactions(dto.Operations, p));
        }

        public async Task<RunnerStatus> StartJob(Func<int, Task<ManyTransactionsContainerDto>> getTransactions)
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
                var initialRequest = await getTransactions(1);
                _status.State = RunnerState.Running;
                _status.CompletedTransactions = 0;
                _completedTransactions = 0;
                _status.TotalTransactions = initialRequest.Meta.Pagination.Total;
                _status.CurrentPage = 1;
                _status.TotalPages = initialRequest.Meta.Pagination.Total_pages;
                _ = Task.Run(() => JobTask(getTransactions, initialRequest, _tokenSource.Token));
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
                _logger.LogInformation("Updated transaction {transactionId}", transaction.Id);
            }
        }

        private async Task JobTask(Func<int, Task<ManyTransactionsContainerDto>> getTransactions, ManyTransactionsContainerDto initialRequest, CancellationToken cancellationToken)
        {
            try
            {
                var currentSet = initialRequest;
                while (true)
                {
                    await Task.WhenAll(currentSet.Data.Select(async t =>
                    {
                        await UpdateTransaction(t, cancellationToken);
                        Interlocked.Increment(ref _completedTransactions);
                    }));

                    _status.CompletedTransactions = _completedTransactions;
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    else if (_status.CurrentPage < _status.TotalPages)
                        _status.CurrentPage++;
                    else
                        break;

                    currentSet = await getTransactions(_status.CurrentPage);
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
