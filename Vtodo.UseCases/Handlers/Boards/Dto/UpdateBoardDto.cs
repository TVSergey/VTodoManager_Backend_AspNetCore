using System.ComponentModel.DataAnnotations;

namespace Vtodo.UseCases.Handlers.Boards.Dto
{
    public class UpdateBoardDto
    {
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int PrioritySort { get; set; }
    }
}