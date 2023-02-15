using Bogus;
using FireflyIIIpp.Core.Exceptions;
using FireflyIIIppRunner.Abstractions;
using FireflyIIIppRunner.Abstractions.Models;
using FireflyIIIppRunner.Abstractions.Models.Dtos;

namespace FireflyIIIpp.Mock.API.Fakes
{
    public class FakeAutoReconcileService : IAutoReconcileService
    {
        private readonly AutoReconcileStatus _status = new AutoReconcileStatus();
        private CancellationTokenSource _tokenSource;

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

        public Task<AutoReconcileStatus> DryRun(AutoReconcileRequestDto dto)
        {
            if (IsJobRunning())
                throw new BusyException();

            ResetStateCounts();
            _status.State = AutoReconcileState.GettingTransactions;
            _tokenSource = new CancellationTokenSource();

            _ = Task.Run(() => DryRunTask(dto, _tokenSource.Token), _tokenSource.Token);

            return Task.FromResult(GetStatus());
        }

        public Task<AutoReconcileStatus> Run(AutoReconcileRequestDto dto)
        {
            if (IsJobRunning())
                throw new BusyException();

            ResetStateCounts();
            _status.State = AutoReconcileState.GettingTransactions;
            _tokenSource = new CancellationTokenSource();

            _ = Task.Run(() => JobTask(dto, _tokenSource.Token), _tokenSource.Token);

            return Task.FromResult(GetStatus());
        }
        public AutoReconcileStatus Stop()
        {
            if (IsJobRunning())
            {
                _tokenSource.Cancel();
                _status.State = AutoReconcileState.Stopped;
            }
            return GetStatus();
        }
        
        public Task<AutoReconcileDryRunResponseDto> GetDryRunResult()
        {
            if (IsJobRunning())
                throw new NotReadyException();

            return Task.FromResult(new AutoReconcileDryRunResponseDto
            {
                Transfers = GenerateTransferData()
            });
        }

        private async Task DryRunTask(AutoReconcileRequestDto dto, CancellationToken token)
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    await Task.Delay(20, token);
                    _status.TotalSourceTransactions++;
                    _status.TotalDestinationTransactions++;
                }
                _status.State = AutoReconcileState.PairingTransactions;
                for (int i = 0; i < 100; i++)
                {
                    await Task.Delay(20, token);
                    _status.TotalTransfers++;
                }
                _status.State = AutoReconcileState.Completed;
            }
            catch (TaskCanceledException)
            {

            }
        }
        private async Task JobTask(AutoReconcileRequestDto dto, CancellationToken token)
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    await Task.Delay(20, token);
                    _status.TotalSourceTransactions++;
                    _status.TotalDestinationTransactions++;
                }
                _status.State = AutoReconcileState.PairingTransactions;
                for (int i = 0; i < 100; i++)
                {
                    await Task.Delay(20, token);
                    _status.TotalTransfers++;
                }
                _status.State = AutoReconcileState.Running;
                for (int i = 0; i < 100; i++)
                {
                    await Task.Delay(20, token);
                    _status.CompletedTransfers++;
                }
                _status.State = AutoReconcileState.Completed;
            }
            catch (TaskCanceledException)
            {

            }
        }

        public AutoReconcileStatus GetStatus() => _status;

        private List<AutoReconcileTransfer> GenerateTransferData()
        {
            return new Faker<AutoReconcileTransfer>()
                .RuleFor(t => t.Source, f => f.Finance.AccountName())
                .RuleFor(t => t.Destination, f => f.Finance.AccountName())
                .RuleFor(t => t.Description, f => f.Commerce.ProductName())
                .RuleFor(t => t.Amount, f => f.Finance.Amount())
                .RuleFor(t => t.Date, f => f.Date.Recent())
                .RuleFor(t => t.Category, f => f.Commerce.Categories(1)[0])
                .RuleFor(t => t.Notes, f => f.PickRandom(f.Lorem.Sentence(), null))
                .RuleFor(t => t.Warning, f => f.PickRandom(f.Lorem.Sentence(), null))
                .Generate(8);
        }

    }
}
