using Bogus;
using FireflyIIIppRunner.Abstractions;
using FireflyIIIppRunner.Abstractions.Models;
using FireflyIIIppRunner.Abstractions.Models.Dtos;

namespace FireflyIIIpp.Mock.API.Fakes
{
    public class FakeAutoReconcileService : IAutoReconcileService
    {
        public Task<AutoReconcileDryRunResponseDto> DryRun(AutoReconcileRequestDto dto)
        {
            return Task.FromResult(new AutoReconcileDryRunResponseDto
            {
                Transfers = GenerateTransferData()
            });
        }

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
