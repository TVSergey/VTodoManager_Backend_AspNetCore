using System.ComponentModel.DataAnnotations;

namespace Vtodo.UseCases.Handlers.Boards.Dto
{
    public class CreateBoardDto
    {
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Title { get; set; } = null!;
        
        [Required]
        public int ProjectId { get; set; }
        
        public int PrioritySort { get; set; }
    }
}