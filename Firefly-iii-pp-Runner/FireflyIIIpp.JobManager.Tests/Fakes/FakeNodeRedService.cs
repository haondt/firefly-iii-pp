﻿using FireflyIIIpp.NodeRed.Abstractions;
using FireflyIIIpp.NodeRed.Abstractions.Models.Dtos;
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
        public Func<string, (bool, string)> Flow { get; set; } = s => (true, s);
        public int Runs { get; private set; } = 0;
        public Task<(bool, string)> TryApplyRules(string input, CancellationToken? cancellationToken = null)
        {
            Runs += 1;
            return Task.FromResult(Flow(input));
        }

        public Task ExportFlows()
        {
            return Task.CompletedTask;
        }

        public Task<NodeRedExtractKeyResponseDto> ExtractKey(string field, string input, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult(new NodeRedExtractKeyResponseDto { Key = input });
        }
    }
}
