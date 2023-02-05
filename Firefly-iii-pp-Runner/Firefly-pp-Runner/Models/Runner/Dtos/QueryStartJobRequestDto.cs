using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Models.Runner.Dtos
{
    public class QueryStartJobRequestDto
    {
        public List<RunnerQueryOperation> Operations { get; set; } = new List<RunnerQueryOperation>();
    }
}
