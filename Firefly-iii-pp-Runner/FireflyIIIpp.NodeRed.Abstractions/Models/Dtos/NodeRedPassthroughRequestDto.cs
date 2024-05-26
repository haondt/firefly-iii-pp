using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.NodeRed.Abstractions.Models.Dtos
{
    public class NodeRedPassthroughRequestDto
    {
        [Required]
        public required string StringifiedJsonPayload { get; set; }
    }
}
