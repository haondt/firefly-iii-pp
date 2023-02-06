using System.ComponentModel.DataAnnotations;

namespace Firefly_pp_Runner.Models.Runner.Dtos
{
    public class RunnerDto
    {
        [Required]
        public DateTime? Start { get; set; } 
        [Required]
        public DateTime? End { get; set; } 
    }

}
