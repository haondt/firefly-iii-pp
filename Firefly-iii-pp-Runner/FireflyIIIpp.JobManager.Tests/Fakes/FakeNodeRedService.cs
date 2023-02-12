using FireflyIIIpp.NodeRed.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireflyIIIpp.Tests.Fakes
{
    public class FakeNodeRedService : INodeRedService
    {
        public Func<string, string> Flow { get; set; } = s => s;
        public int Runs { get; private set; } = 0;
        public Task<string> ApplyRules(string input, CancellationToken? cancellationToken = null)
        {
            Runs += 1;
            return Task.FromResult(Flow(input));
        }

        public Task ExportFlows()
        {
            return Task.CompletedTask;
        }
    }
}
