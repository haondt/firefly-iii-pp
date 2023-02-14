using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIppRunner.Abstractions.Models.Dtos
{
    public class AutoReconcileDryRunResponseDto
    {
        public List<AutoReconcileTransfer> Transfers { get; set; } = new List<AutoReconcileTransfer>();
    }
}
