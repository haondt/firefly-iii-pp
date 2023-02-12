using FireflyIIIpp.Core.Models;
using FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Models.Runner.Dtos
{
    public class DryRunResponseDto
    {
        public List<RunnerQueryOperation> Operations { get; set; } = new List<RunnerQueryOperation>();
        public string Query { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalPages { get; set; }
        public object Sample { get; set; }
    }
}
