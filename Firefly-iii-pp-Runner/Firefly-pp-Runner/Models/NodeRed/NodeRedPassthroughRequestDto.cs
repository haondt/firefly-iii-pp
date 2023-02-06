using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Models.NodeRed
{
    public class NodeRedPassthroughRequestDto
    {
        [Required]
        public string StringifiedJsonPayload { get; set; }
    }
}
