using Firefly_iii_pp_Runner.Exceptions;
using Firefly_iii_pp_Runner.Models;
using Firefly_pp_Runner.Extensions;
using Firefly_pp_Runner.Models.Runner;
using Firefly_pp_Runner.Models.Runner.Dtos;
using FireflyIIIpp.Core.Extensions;
using FireflyIIIpp.FireflyIII.Abstractions;
using FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos;
using FireflyIIIpp.NodeRed.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Firefly_iii_pp_Runner.Services
{
    public class JobManager
    {
        private readonly ILogger<JobManager> _logger;
        private readonly IFireflyIIIService _fireflyIII;
        private readonly INodeRedService _nodeRed;
        private readonly RunnerStatus _status;
        private CancellationTokenSource _tokenSource;
        private JsonSerializerSettings _serializerSettings;

        public JobManager(ILogger<JobManager> logger, IFireflyIIIService fireflyIII, INodeRedService nodeRed)
        {
            _logger = logger;
            _fireflyIII = fireflyIII;
            _nodeRed = nodeRed;
            _status = new RunnerStatus();
            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.ConfigureFireflyppRunnerSettings();
        }

        public RunnerStatus GetStatus()
        {
            return _status;
        }

        public async Task<RunnerStatus> StartSingle(RunnerSingleDto dto)
        {
            if (IsJobRunning())
                throw new RunnerBusyException();

            if (_tokenSource != null)
                _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
            try
            {
                _status.State = RunnerState.GettingTransactions;
                _status.CompletedTransactions = 0;
                _status.TotalTransactions = 0;
                _status.CurrentPage = 1;
                _status.TotalPages = 1;

                var initialRequest = await _fireflyIII.GetTransaction(dto.Id);

                if (initialRequest.Attributes.Transactions.Count != 1)
                    throw new InvalidOperationException("Multi-part transaction");

                _status.State = RunnerState.RunningTransactions;
                _status.TotalTransactions = 1;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await UpdateTransaction(initialRequest, _tokenSource.Token);
                        _status.State = RunnerState.Completed;
                        _status.CompletedTransactions = 1;
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

        public Task<RunnerStatus> StartJob(QueryStartJobRequestDto dto, Func<Task>? onJobFinish = null)
        {
            return StartJob(p => _fireflyIII.GetTransactions(dto.Operations, p), onJobFinish);
        }

        private async Task<RunnerStatus> StartJob(Func<int, Task<ManyTransactionsContainerDto>> getTransactions, Func<Task>? onJobFinish = null)
        {
            if (IsJobRunning())
                throw new RunnerBusyException();

            if (_tokenSource != null)
                _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
            try
            {
                _status.State = RunnerState.GettingTransactions;
                _status.CompletedTransactions = 0;
                _status.TotalTransactions = 0;
                _status.CurrentPage = 1;
                _status.TotalPages = 1;

                var initialRequest = await getTransactions(1);
                _status.TotalTransactions = initialRequest.Meta.Pagination.Total;
                _status.TotalPages = initialRequest.Meta.Pagination.Total_pages;

                _ = Task.Run(() => JobTask(getTransactions, initialRequest, _tokenSource.Token, onJobFinish));
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
            var transactionDataString = JsonConvert.SerializeObject(transactionData, _serializerSettings); ;
            var newTransactionDataString = await _nodeRed.ApplyRules(transactionDataString, cancellationToken);
            var newTransactionData = JsonConvert.DeserializeObject<TransactionPartDto>(newTransactionDataString, _serializerSettings);
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

        private async Task JobTask(Func<int, Task<ManyTransactionsContainerDto>> getTransactions, ManyTransactionsContainerDto initialRequest, CancellationToken cancellationToken, Func<Task>? onJobFinish = null)
        {
            try
            {
                var transactionIds = new List<string>();

                var currentSet = initialRequest;
                while (true)
                {
                    transactionIds.AddRange(currentSet.Data.Select(t => t.Id));
                    _status.TotalTransactions = transactionIds.Count;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        _status.State = RunnerState.Stopped;
                        break;
                    }
                    else if (_status.CurrentPage < _status.TotalPages)
                        _status.CurrentPage++;
                    else
                        break;

                    currentSet = await getTransactions(_status.CurrentPage);
                }

                _status.State = RunnerState.RunningTransactions;

                foreach(var page in transactionIds.Paginate(10))
                {
                    await Task.WhenAll(page.Select(async id =>
                    {
                        var transaction = await _fireflyIII.GetTransaction(id);
                        await UpdateTransaction(transaction, cancellationToken);
                        lock(_status)
                        {
                            _status.CompletedTransactions++;
                        }
                    }));
                }

                _status.State = RunnerState.Completed;

                if (onJobFinish != null)
                    await onJobFinish();
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

        private bool IsJobRunning()
        {
            return !(_status.State == RunnerState.Failed
                || _status.State == RunnerState.Completed
                || _status.State == RunnerState.Stopped);
        }

        public RunnerStatus StopJob()
        {
            if (IsJobRunning())
            {
                _tokenSource.Cancel();
                _status.State = RunnerState.Stopped;
            }
            return _status;
        }
    }
}
