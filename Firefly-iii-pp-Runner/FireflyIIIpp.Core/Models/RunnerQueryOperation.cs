using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.Core.Models
{
    public class RunnerQueryOperation
    {
        [Required]
        public string Operand { get; set; }
        [Required]
        public string Operator { get; set; }
        [Required]
        public object Result { get; set; }
    }
}
