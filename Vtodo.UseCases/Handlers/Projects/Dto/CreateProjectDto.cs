using System.ComponentModel.DataAnnotations;

namespace Vtodo.UseCases.Handlers.Projects.Dto
{
    public class CreateProjectDto
    {
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Title { get; set; } = null!;
    }
}