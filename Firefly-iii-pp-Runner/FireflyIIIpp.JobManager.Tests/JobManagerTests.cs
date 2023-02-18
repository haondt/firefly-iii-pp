using Xunit;
using Firefly_iii_pp_Runner.Services;
using FireflyIIIpp.Core.Tests.Utilities;
using FireflyIIIpp.Tests.Fakes;
using FireflyIIIpp.NodeRed.Abstractions;
using Firefly_pp_Runner.Models.Runner.Dtos;
using System.Collections.Generic;
using FireflyIIIpp.Core.Models;
using FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos;
using System;
using FireflyIIIpp.FireflyIII.Abstractions.Models;
using Newtonsoft.Json;
using Firefly_pp_Runner.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace FireflyIIIpp.Tests
{
    public class JobManagerTests
    {
        private readonly JobManager _sut;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly FakeFireflyIIIService _fireflyIIIService;
        private readonly FakeNodeRedService _nodeRedService;

        public JobManagerTests()
        {
            _fireflyIIIService = new FakeFireflyIIIService
            {
                PageSize = 5
            };
            _nodeRedService = new FakeNodeRedService();
            _sut = new JobManager(
                new FakeLogger<JobManager>(),
                _fireflyIIIService,
                _nodeRedService);
            _serializerSettings = new JsonSerializerSettings().ConfigureFireflyppRunnerSettings();
        }

        [Fact]
        public async Task ShouldNotSkipTransactionsWhenQueryResultsChange()
        {
            _fireflyIIIService.Transactions = new Dictionary<string, TransactionDto>();
            for(int i=1; i<11; i++)
            {
                var id = Guid.NewGuid().ToString();
                _fireflyIIIService.Transactions[id] = new TransactionDto
                {
                    Id = id,
                    Attributes = new TransactionAttributes
                    {
                        Transactions = new List<TransactionPartDto>
                        {
                            new TransactionPartDto
                            {
                               Description = i.ToString(),
                            }
                        }
                    }
                };
            }

            _fireflyIIIService.Query = t =>
            {
                var num = int.Parse(t.Attributes.Transactions.Single().Description);
                return num < 100;
            };

            _nodeRedService.Flow = s =>
            {
                var t = JsonConvert.DeserializeObject<TransactionPartDto>(s);
                if (t.Description == "2" || t.Description == "4" )
                    t.Description = (int.Parse(t.Description) * 100).ToString();
                return JsonConvert.SerializeObject(t);
            };

            var taskCompletionSource = new TaskCompletionSource();

            await _sut.StartJob(new QueryStartJobRequestDto(), async () => taskCompletionSource.TrySetResult());

            Assert.Equal(await Task.WhenAny(taskCompletionSource.Task, Task.Delay(1000)), taskCompletionSource.Task);
            Assert.Equal(2, _fireflyIIIService.UpdatedTransactions.Count);
            Assert.Equal(2, _sut.GetStatus().TotalPages);
            Assert.Equal(10, _sut.GetStatus().TotalTransactions);
            Assert.Equal(10, _sut.GetStatus().CompletedTransactions);
            Assert.Equal(10, _nodeRedService.Runs);
        }
    }
}