using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.Core.Models.Dtos
{
    public class ExceptionDto
    {
        public int StatusCode { get; set; }
        public string? Exception { get; set; }
        public string? Message { get; set; }
        public string? Details { get; set; }
    }
}
