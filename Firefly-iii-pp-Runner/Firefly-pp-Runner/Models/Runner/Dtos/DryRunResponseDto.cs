using Firefly_iii_pp_Runner.Models.FireflyIII;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Models.Runner.Dtos
{
    public class DryRunResponseDto
    {
        public List<RunnerQueryOperator> Operators { get; set; } = new List<RunnerQueryOperator>();
        public string Query { get; set; }
        public int TotalTranasactions { get; set; }
        public int TotalPages { get; set; }
        public TransactionPartDto? Sample { get; set; }
    }
}
