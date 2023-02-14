using FireflyIIIppRunner.Abstractions.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIppRunner.Abstractions
{
    public interface IAutoReconcileService
    {
        public Task<AutoReconcileDryRunResponseDto> DryRun(AutoReconcileRequestDto dto);
    }
}
