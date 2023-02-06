using System.ComponentModel.DataAnnotations;

namespace Firefly_pp_Runner.Models.Runner.Dtos
{
    public class RunnerSingleDto
    {
        [Required]
        public string Id { get; set; }
    }
}
