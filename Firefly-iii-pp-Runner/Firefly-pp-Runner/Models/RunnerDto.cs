using System.ComponentModel.DataAnnotations;

namespace Firefly_iii_pp_Runner.Models
{
    public class RunnerDto
    {
        [Required]
        public DateTime? Start { get; set; } 
        [Required]
        public DateTime? End { get; set; } 
    }

}
