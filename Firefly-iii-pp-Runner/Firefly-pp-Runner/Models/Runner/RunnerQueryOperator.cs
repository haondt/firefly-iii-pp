using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Models.Runner
{
    public class RunnerQueryOperator
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Option { get; set; }
        [Required]
        public object Value { get; set; }
    }
}
