using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Models.Lookup.Dtos
{
    public class LookupActionRequestDto
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
        public string PartialValue { get; set; } = string.Empty;
        public string? ValueValue { get; set; }
    }
}
