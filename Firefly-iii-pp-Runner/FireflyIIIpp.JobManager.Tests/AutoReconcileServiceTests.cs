using Firefly_pp_Runner.Services;
using FireflyIIIpp.Core.Models;
using FireflyIIIpp.Core.Tests.Utilities;
using FireflyIIIpp.FireflyIII.Abstractions.Models;
using FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos;
using FireflyIIIpp.Tests.Fakes;
using FireflyIIIppRunner.Abstractions.AutoReconcile.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FireflyIIIpp.Tests
{
    public class AutoReconcileServiceTests
    {
        private readonly FakeFireflyIIIService _fireflyIIIService;
        private readonly AutoReconcileService _sut;

        public AutoReconcileServiceTests()
        {
            _fireflyIIIService = new FakeFireflyIIIService
            {
                PageSize = 5
            };
            _sut = new AutoReconcileService(_fireflyIIIService, new FakeLogger<AutoReconcileService>());
        }

        private TransactionDto CreateTransactionDto(string id, TransactionPartDto partDto)
        {
            return new TransactionDto
            {
                Id = id,
                Attributes = new TransactionAttributes
                {
                    Transactions = new List<TransactionPartDto> { partDto }
                }
            };
        }

        private Func<TransactionDto, bool> CreateTransactionPartFilter(Func<TransactionPartDto, bool> func)
        {
            return t => func(t.Attributes.Transactions.Single());
        }

        [Fact]
        public async Task ShouldPairTransactionsWhenOnlyTwoAvailable()
        {
            _fireflyIIIService.Transactions = new Dictionary<string, TransactionDto>
            {
                { "1", CreateTransactionDto("1", new TransactionPartDto
                {
                    Description = "source tx",
                    Amount = "100",
                    Type = "withdrawal",
                    Source_name = "Acc1",
                    Destination_name = "(no name)",
                    Date = DateTime.Now.ToString("o", CultureInfo.InvariantCulture)

                }) },
                { "2", CreateTransactionDto("2", new TransactionPartDto
                {
                    Description = "dest tx",
                    Amount = "100",
                    Type = "deposit",
                    Source_name = "(no name)",
                    Destination_name = "Acc2",
                    Date = DateTime.Now.ToString("o", CultureInfo.InvariantCulture)

                }) }
            };

            _fireflyIIIService.AddQuery(CreateTransactionPartFilter(t => t.Description == "source tx"));
            _fireflyIIIService.AddQuery(CreateTransactionPartFilter(t => t.Description == "dest tx"));

            var status = await _sut.DryRun(new AutoReconcileRequestDto
            {
                PairingStrategy = new AutoReconcilePairingStrategyDto
                {
                    RequireMatchingDates = false,
                    RequireMatchingDescriptions = false
                }
            });

            var maxWaits = 5;
            var waits = 0;
            while(!(status.State == AutoReconcileState.Failed || status.State == AutoReconcileState.Completed || status.State == AutoReconcileState.Stopped))
            {
                Assert.True(waits < maxWaits);
                await Task.Delay(100);
                status = _sut.GetStatus();
                waits++;
            }

            Assert.Equal(AutoReconcileState.Completed, status.State);

            var result = await _sut.GetDryRunResult();

            Assert.Collection(result.Transfers, _ => { });
        }

        public async Task ShouldPairTransactionsBasedOnType()
        {

        }

        [Fact]
        public async Task ShouldPairTransactionsBasedOnDateWindow()
        {
            var now = DateTime.Now;
            _fireflyIIIService.Transactions = new Dictionary<string, TransactionDto>
            {
                { "3", CreateTransactionDto("3", new TransactionPartDto
                {
                    Description = "source tx",
                    Amount = "100",
                    Type = "withdrawal",
                    Source_name = "Acc1",
                    Destination_name = "(no name)",
                    Date = now.ToString("o", CultureInfo.InvariantCulture)

                }) },
                { "1", CreateTransactionDto("1", new TransactionPartDto
                {
                    Description = "source tx",
                    Amount = "100",
                    Type = "withdrawal",
                    Source_name = "Acc1",
                    Destination_name = "(no name)",
                    Date = (now + TimeSpan.FromDays(7)).ToString("o", CultureInfo.InvariantCulture)

                }) },
                { "2", CreateTransactionDto("2", new TransactionPartDto
                {
                    Description = "dest tx",
                    Amount = "100",
                    Type = "deposit",
                    Source_name = "(no name)",
                    Destination_name = "Acc2",
                    Date = ((now + TimeSpan.FromDays(8))).ToString("o", CultureInfo.InvariantCulture)

                }) }
            };

            _fireflyIIIService.AddQuery(CreateTransactionPartFilter(t => t.Description == "source tx"));
            _fireflyIIIService.AddQuery(CreateTransactionPartFilter(t => t.Description == "dest tx"));

            var status = await _sut.DryRun(new AutoReconcileRequestDto
            {
                PairingStrategy = new AutoReconcilePairingStrategyDto
                {
                    RequireMatchingDates = true,
                    DateMatchToleranceInDays = 2,
                    RequireMatchingDescriptions = false
                }
            });

            var maxWaits = 5;
            var waits = 0;
            while(!(status.State == AutoReconcileState.Failed || status.State == AutoReconcileState.Completed || status.State == AutoReconcileState.Stopped))
            {
                Assert.True(waits < maxWaits);
                await Task.Delay(100);
                status = _sut.GetStatus();
                waits++;
            }

            Assert.Equal(AutoReconcileState.Completed, status.State);

            var result = await _sut.GetDryRunResult();

            Assert.Collection(result.Transfers, _ => { });

        }

    }
}
