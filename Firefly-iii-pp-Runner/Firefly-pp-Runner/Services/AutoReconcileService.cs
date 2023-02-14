using Firefly_pp_Runner.Models.AutoReconcile;
using Firefly_pp_Runner.Models.AutoReconcile.Dtos;
using Firefly_pp_Runner.Models.AutoReconcile.Enums;
using FireflyIIIpp.Core.Models;
using FireflyIIIpp.FireflyIII.Abstractions;
using FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Services
{
    public class AutoReconcileService
    {
        private readonly IFireflyIIIService _fireflyIIIService;

        public AutoReconcileService(IFireflyIIIService fireflyIIIService)
        {
            _fireflyIIIService = fireflyIIIService;
        }

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

        public async Task<AutoReconcileDryRunResponseDto> DryRun(AutoReconcileRequestDto dto)
        {
            var hashingStrategy = BuildHashingStrategy(dto.PairingStrategy);
            var joiningStrategy = BuildJoiningStrategy(dto.JoiningStrategy);
            var sourceTransactions = await GetTransactions(dto.SourceQueryOperations);
            var destinationTransactions = await GetTransactions(dto.DestinationQueryOperations);
            var groupedSourceTransactions = sourceTransactions
                .GroupBy(hashingStrategy)
                .Where(grp => grp.Count() == 1)
                .ToDictionary(grp => grp.Key, grp => grp.Single());
            var groupedDestinationTransactions = destinationTransactions
                .GroupBy(hashingStrategy)
                .Where(grp => grp.Count() == 1)
                .ToDictionary(grp => grp.Key, grp => grp.Single());
            var transferPairs = groupedSourceTransactions
                .Where(kvp => groupedDestinationTransactions.ContainsKey(kvp.Key))
                .Select<KeyValuePair<int, TransactionPartDto>, (TransactionPartDto Source, TransactionPartDto Destination)>(kvp => (kvp.Value, groupedDestinationTransactions[kvp.Key]))
                .Where(p => p.Source.Source_name != p.Destination.Destination_name);

            if (dto.PairingStrategy.RequireMatchingDates && dto.PairingStrategy.DateMatchToleranceInDays > 0)
                transferPairs = transferPairs
                    .Where(p => Math.Abs((DateTime.Parse(p.Source.Date) - DateTime.Parse(p.Destination.Date)).TotalDays) <= dto.PairingStrategy.DateMatchToleranceInDays);

            var transfers = transferPairs
                .Select(p => joiningStrategy(p.Source, p.Destination))
                .ToList();

            return new AutoReconcileDryRunResponseDto
            {
                Transfers = transfers
            };
        }

        public async Task<List<TransactionPartDto>> GetTransactions(List<RunnerQueryOperation> query)
        {
            var currentSet = await _fireflyIIIService.GetTransactions(query, 1);
            if (currentSet.Data.Count == 0)
                return new List<TransactionPartDto>();

            var transactions = new List<TransactionPartDto>();
            transactions.AddRange(currentSet.Data
                .Select(x => x.Attributes.Transactions)
                .Where(x => x.Count == 1)
                .Select(x => x.Single()));
            var finalPage = currentSet.Meta.Pagination.Total_pages;
            var currentPage = currentSet.Meta.Pagination.Current_page;

            while(currentPage < finalPage)
            {
                currentPage++;
                currentSet = await _fireflyIIIService.GetTransactions(query, currentPage);
                transactions.AddRange(currentSet.Data
                    .Select(x => x.Attributes.Transactions)
                    .Where(x => x.Count == 1)
                    .Select(x => x.Single()));
            }

            return transactions;
        }

    }
}
