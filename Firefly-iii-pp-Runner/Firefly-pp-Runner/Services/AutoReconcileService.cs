using FireflyIIIpp.Core.Exceptions;
using FireflyIIIpp.Core.Models;
using FireflyIIIpp.FireflyIII.Abstractions;
using FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos;
using FireflyIIIppRunner.Abstractions;
using FireflyIIIppRunner.Abstractions.AutoReconcile;
using FireflyIIIppRunner.Abstractions.AutoReconcile.Models;
using FireflyIIIppRunner.Abstractions.AutoReconcile.Models.Dtos;
using FireflyIIIppRunner.Abstractions.AutoReconcile.Models.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Services
{
    public class AutoReconcileService : IAutoReconcileService
    {
        private readonly IFireflyIIIService _fireflyIIIService;
        private readonly ILogger<AutoReconcileService> _logger;
        private readonly AutoReconcileStatus _status = new AutoReconcileStatus();
        private AutoReconcileDryRunResponseDto _dryRunResult;
        private CancellationTokenSource _tokenSource;

        public AutoReconcileService(IFireflyIIIService fireflyIIIService, ILogger<AutoReconcileService> logger)
        {
            _fireflyIIIService = fireflyIIIService;
            _logger = logger;
        }

        #region utils

        private Func<TransactionPartDto, int> BuildHashingStrategy(AutoReconcilePairingStrategyDto dto)
        {
            return t =>
            {
                var hc = new HashCode();
                hc.Add(decimal.Parse(t.Amount));
                if (dto.RequireMatchingDescriptions)
                    hc.Add(t.Description);
                if (dto.RequireMatchingDates && dto.DateMatchToleranceInDays == 0)
                    hc.Add(DateTime.Parse(t.Date));
                return hc.ToHashCode();
            };
        }

        private Func<TransactionPartDto, TransactionPartDto, AutoReconcileTransfer> BuildJoiningStrategy(AutoReconcileJoiningStrategyDto dto)
        {
            return (source, destination) =>
            {
                var transfer = new AutoReconcileTransfer
                {
                    Source = source.Source_name,
                    Destination = destination.Destination_name,
                    Amount = decimal.Parse(source.Amount)
                };

                switch (dto.DescriptionJoinStrategy)
                {
                    case JoiningStrategyEnum.Source:
                        transfer.Description = source.Description;
                        break;
                    case JoiningStrategyEnum.Destination:
                        transfer.Description = destination.Description;
                        break;
                    case JoiningStrategyEnum.Concatenate:
                        if(!string.IsNullOrEmpty(source.Description) && !string.IsNullOrEmpty(destination.Description))
                            transfer.Description = source.Description + " | " + destination.Description;
                        break;
                    default:
                        throw new ArgumentException(nameof(dto.DescriptionJoinStrategy));
                }

                var sourceDate = DateTime.Parse(source.Date);
                var destinationDate = DateTime.Parse(destination.Date);
                switch (dto.DateJoinStrategy)
                {
                    case JoiningStrategyEnum.Source:
                        transfer.Date = sourceDate;
                        break;
                    case JoiningStrategyEnum.Destination:
                        transfer.Date = destinationDate;
                        break;
                    case JoiningStrategyEnum.Average:
                        transfer.Date = new DateTime(((sourceDate.Ticks - destinationDate.Ticks) / 2) + destinationDate.Ticks);
                        break;
                    default:
                        throw new ArgumentException(nameof(dto.DateJoinStrategy));
                }
                if (sourceDate != destinationDate)
                {
                    transfer.Warning = $"Source and destination dates are {Math.Abs((sourceDate - destinationDate).TotalDays)} Days apart.";
                }

                switch (dto.CategoryJoinStrategy)
                {
                    case JoiningStrategyEnum.Source:
                        transfer.Category = source.Category_name;
                        break;
                    case JoiningStrategyEnum.Destination:
                        transfer.Category = destination.Category_name;
                        break;
                    case JoiningStrategyEnum.Clear:
                        transfer.Category = null;
                        break;
                    default:
                        throw new ArgumentException(nameof(dto.CategoryJoinStrategy));
                }

                switch (dto.NotesJoinStrategy)
                {
                    case JoiningStrategyEnum.Source:
                        transfer.Notes = source.Notes;
                        break;
                    case JoiningStrategyEnum.Destination:
                        transfer.Notes = destination.Notes;
                        break;
                    case JoiningStrategyEnum.Concatenate:
                        if(!string.IsNullOrEmpty(source.Notes) && !string.IsNullOrEmpty(destination.Notes))
                            transfer.Notes = source.Notes + " | " + destination.Notes;
                        break;
                    default:
                        throw new ArgumentException(nameof(dto.NotesJoinStrategy));
                }


                return transfer;
            };
        }

        private bool IsJobRunning()
        {
            return !(_status.State == AutoReconcileState.Failed
                || _status.State == AutoReconcileState.Completed
                || _status.State == AutoReconcileState.Stopped);
        }

        private void ResetStateCounts()
        {
            _status.TotalTransfers = 0;
            _status.TotalSourceTransactions = 0;
            _status.TotalDestinationTransactions = 0;
            _status.CompletedTransfers = 0;
        }

        private async Task<(List<(string Id, TransactionPartDto)> SourceTransactions, List<(string Id, TransactionPartDto)> DestinationTransactions)> GetTransactions(AutoReconcileRequestDto dto, CancellationToken cancellationToken)
        {
            var sourceTransactions = await GetTransactions(dto.SourceQueryOperations, cancellationToken);
            var destinationTransactions = await GetTransactions(dto.DestinationQueryOperations, cancellationToken);
            return (sourceTransactions, destinationTransactions);
        }

        private List<(AutoReconcileTransfer Transfer, string SourceTransactionId, string DestinationTransactionId)> PairTransactions(
            AutoReconcilePairingStrategyDto pairingStrategy,
            Func<TransactionPartDto, int> hashingStrategy,
            Func<TransactionPartDto, TransactionPartDto, AutoReconcileTransfer> joiningStrategy,
            List<(string Id, TransactionPartDto Transaction)> sourceTransactions,
            List<(string Id, TransactionPartDto Transaction)> destinationTransactions)
        {
            var groupedSourceTransactions = sourceTransactions
                .GroupBy(t => hashingStrategy(t.Transaction))
                .ToDictionary(grp => grp.Key, grp => grp.ToList());
            var groupedDestinationTransactions = destinationTransactions
                .GroupBy(t => hashingStrategy(t.Transaction))
                .ToDictionary(grp => grp.Key, grp => grp.ToList());
            var transferPairs = groupedSourceTransactions
                .Where(kvp => groupedDestinationTransactions.ContainsKey(kvp.Key))
                .Select(kvp => (kvp.Value, groupedDestinationTransactions[kvp.Key]))
                .Select(t =>
                {
                    var (sts, dts) = t;
                    var result = new List<((string Id, TransactionPartDto Transaction) Source, (string Id, TransactionPartDto Transaction) Destination)>();
                    if (!pairingStrategy.RequireMatchingDates)
                    {
                        if (sts.Count == 1 && dts.Count == 1)
                            result.Add((sts[0], dts[0]));
                        return result;
                    }

                    if (pairingStrategy.DateMatchToleranceInDays == 0)
                    {
                        var stsd = sts
                        .GroupBy(p => p.Transaction.Date)
                        .Where(p => p.Count() == 1)
                        .ToDictionary(grp => grp.Key, grp => grp.Single());
                        var _dtsd = dts.GroupBy(p => p.Transaction.Date)
                        .Where(p => p.Count() == 1)
                        .ToDictionary(grp => grp.Key, grp => grp.Single());
                        result.AddRange(stsd.Where(p => _dtsd.ContainsKey(p.Key)).Select(p => (p.Value, _dtsd[p.Key])));
                        return result;
                    }


                    var sdd = new Dictionary<string, string?>();
                    var sWindow = new Queue<(DateTime Date, string Id, TransactionPartDto Transaction)>();
                    var dWindow = new Queue<(DateTime Date, string Id, TransactionPartDto Transaction)>();
                    var nextWindowStart = new Queue<DateTime>();
                    var allTransactions = new Queue<(DateTime Date, bool IsSource, string Id, TransactionPartDto Transaction)>(sts.Select(s => (DateTime.Parse(s.Transaction.Date), true, s.Id, s.Transaction)).Concat(dts.Select(d => (DateTime.Parse(d.Transaction.Date), false, d.Id, d.Transaction)))
                        .OrderBy(t => t.Item1));
                    var windowStart = allTransactions.Peek().Date;
                    var windowEnd = windowStart + TimeSpan.FromDays(pairingStrategy.DateMatchToleranceInDays);

                    while (allTransactions.Count > 0)
                    {
                        while (allTransactions.Count > 0 && allTransactions.Peek().Date <= windowEnd)
                        {
                            var (date, isSource, id, transaction) = allTransactions.Dequeue();
                            if (isSource)
                                sWindow.Enqueue((date, id, transaction));
                            else
                                dWindow.Enqueue((date, id, transaction));
                            if (allTransactions.Count > 0)
                                nextWindowStart.Enqueue(allTransactions.Peek().Date);
                        }


                        if (sWindow.Count == 0 || dWindow.Count == 0)
                        {
                        }
                        else if (sWindow.Count == 1 && dWindow.Count == 1)
                        {
                            var sId = sWindow.Peek().Id;
                            var dId = dWindow.Peek().Id;
                            sdd[sId] = sdd.ContainsKey(sId) ? null : dId;
                            sdd[dId] = sdd.ContainsKey(dId) ? null : sId;
                        }
                        else
                        {
                            foreach (var (_, sId, _) in sWindow)
                                sdd[sId] = null;
                            foreach (var (_, dId, _) in dWindow)
                                sdd[dId] = null;
                        }


                        while (nextWindowStart.Count > 0 && nextWindowStart.Peek().Date == windowStart)
                            nextWindowStart.Dequeue();
                        if (nextWindowStart.Count == 0)
                            break;

                        windowStart = nextWindowStart.Dequeue();
                        windowEnd = windowStart + TimeSpan.FromDays(pairingStrategy.DateMatchToleranceInDays);
                        while (sWindow.Count > 0 && sWindow.Peek().Date < windowStart)
                            sWindow.Dequeue();
                        while (dWindow.Count > 0 && dWindow.Peek().Date < windowStart)
                            dWindow.Dequeue();
                    }

                    var dtsd = dts.ToDictionary(q => q.Id, q => q);
                    foreach (var s in sts)
                    {
                        if (sdd.TryGetValue(s.Id, out var dId) && dId != null)
                        {
                            if (sdd.TryGetValue(dId, out var sId) && sId != null && sId == s.Id)
                            {
                                result.Add((s, dtsd[dId]));
                            }
                        }
                    }

                    return result;
                })
                .SelectMany(t => t)
                .Where(p => p.Source.Transaction.Source_name != p.Destination.Transaction.Destination_name)

                // sanity check
                .DistinctBy(p => p.Source.Id)
                .DistinctBy(p => p.Destination.Id);


            var transfers = transferPairs
                .Select(p => (joiningStrategy(p.Source.Transaction, p.Destination.Transaction), p.Source.Id, p.Destination.Id))
                .ToList();

            return transfers;
        }

        #endregion

        #region tasks

        private async Task DryRunTask(
            AutoReconcileRequestDto dto,
            Func<TransactionPartDto, int> hashingStrategy, 
            Func<TransactionPartDto, TransactionPartDto, AutoReconcileTransfer> joiningStrategy,
            CancellationToken cancellationToken)
        {
            try
            {
                var (sourceTransactions, destinationTransactions) = await GetTransactions(dto, cancellationToken);
                _status.TotalSourceTransactions = sourceTransactions.Count;
                _status.TotalDestinationTransactions = destinationTransactions.Count;
                _status.State = AutoReconcileState.PairingTransactions;
                var transfers = PairTransactions(dto.PairingStrategy, hashingStrategy, joiningStrategy, sourceTransactions, destinationTransactions);
                _dryRunResult = new AutoReconcileDryRunResponseDto { Transfers = transfers.Select(t => t.Transfer).ToList() };
                _status.TotalTransfers = transfers.Count;
                _status.State = AutoReconcileState.Completed;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Dry run task was cancelled.");
                _status.State = AutoReconcileState.Stopped;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dry run task failed.");
                _status.State = AutoReconcileState.Failed;
            }
        }

        private async Task RunTask(
            AutoReconcileRequestDto dto,
            Func<TransactionPartDto, int> hashingStrategy, 
            Func<TransactionPartDto, TransactionPartDto, AutoReconcileTransfer> joiningStrategy,
            CancellationToken cancellationToken)
        {
            try
            {
                var (sourceTransactions, destinationTransactions) = await GetTransactions(dto, cancellationToken);
                _status.TotalSourceTransactions = sourceTransactions.Count;
                _status.TotalDestinationTransactions = destinationTransactions.Count;

                _status.State = AutoReconcileState.PairingTransactions;
                var transfers = PairTransactions(dto.PairingStrategy, hashingStrategy, joiningStrategy, sourceTransactions, destinationTransactions);
                _status.TotalTransfers = transfers.Count;

                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();

                _status.State = AutoReconcileState.Running;
                foreach(var transfer in transfers)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException();
                    await _fireflyIIIService.CreateTransaction(new CreateTransactionDto
                    {
                        Transactions = new List<TransactionPartDto>
                        {
                            new TransactionPartDto
                            {
                                Source_name = transfer.Transfer.Source,
                                Destination_name = transfer.Transfer.Destination,
                                Description = transfer.Transfer.Description,
                                Amount = transfer.Transfer.Amount.ToString(),
                                Date = transfer.Transfer.Date.ToString("o", CultureInfo.InvariantCulture),
                                Category_name = transfer.Transfer.Category,
                                Notes = transfer.Transfer.Notes,
                                Type = "transfer"
                            }
                        }
                    });

                    await _fireflyIIIService.DeleteTransaction(transfer.SourceTransactionId);
                    await _fireflyIIIService.DeleteTransaction(transfer.DestinationTransactionId);
                    _status.CompletedTransfers++;
                }

                _status.State = AutoReconcileState.Completed;

            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Run task was cancelled.");
                _status.State = AutoReconcileState.Stopped;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Run task failed.");
                _status.State = AutoReconcileState.Failed;
            }
        }

        #endregion

        #region api

        public AutoReconcileStatus GetStatus() => _status;

        public Task<AutoReconcileStatus> DryRun(AutoReconcileRequestDto dto)
        {
            if (IsJobRunning())
                throw new BusyException();

            ResetStateCounts();
            _status.State = AutoReconcileState.GettingTransactions;
            var hashingStrategy = BuildHashingStrategy(dto.PairingStrategy);
            var joiningStrategy = BuildJoiningStrategy(dto.JoiningStrategy);
            _tokenSource = new CancellationTokenSource();

            _ = Task.Run(() => DryRunTask(dto, hashingStrategy, joiningStrategy, _tokenSource.Token));

            return Task.FromResult(_status);
        }
        public Task<AutoReconcileStatus> Run(AutoReconcileRequestDto dto)
        {
            if (IsJobRunning())
                throw new BusyException();

            ResetStateCounts();
            _status.State = AutoReconcileState.GettingTransactions;
            var hashingStrategy = BuildHashingStrategy(dto.PairingStrategy);
            var joiningStrategy = BuildJoiningStrategy(dto.JoiningStrategy);
            _tokenSource = new CancellationTokenSource();

            _ = Task.Run(() => RunTask(dto, hashingStrategy, joiningStrategy, _tokenSource.Token));

            return Task.FromResult(_status);
        }

        public AutoReconcileStatus Stop()
        {
            if (IsJobRunning())
            {
                _tokenSource.Cancel();
                _status.State = AutoReconcileState.Stopped;
            }
            return _status;
        }

        public Task<AutoReconcileDryRunResponseDto> GetDryRunResult()
        {
            if (IsJobRunning() || _dryRunResult == null)
                throw new NotReadyException();
            return Task.FromResult(_dryRunResult);
        }


        #endregion

        #region firefly-iii

        private async Task<List<(string Id, TransactionPartDto Transaction)>> GetTransactions(List<RunnerQueryOperation> query, CancellationToken cancellationToken)
        {
            var currentSet = await _fireflyIIIService.GetTransactions(query, 1);
            if (currentSet.Data.Count == 0)
                return new List<(string Id, TransactionPartDto Transaction)>();

            var transactions = new List<(string Id, TransactionPartDto Transaction)>();
            transactions.AddRange(currentSet.Data
                .Select(x => (x.Id, x.Attributes.Transactions))
                .Where(x => x.Transactions.Count == 1)
                .Select(x => (x.Id, x.Transactions.Single())));
            var finalPage = currentSet.Meta.Pagination.Total_pages;
            var currentPage = currentSet.Meta.Pagination.Current_page;

            while(currentPage < finalPage)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();

                currentPage++;
                currentSet = await _fireflyIIIService.GetTransactions(query, currentPage);
                transactions.AddRange(currentSet.Data
                    .Select(x => (x.Id, x.Attributes.Transactions))
                    .Where(x => x.Transactions.Count == 1)
                    .Select(x => (x.Id, x.Transactions.Single())));
            }

            return transactions;
        }

        #endregion

    }
}
