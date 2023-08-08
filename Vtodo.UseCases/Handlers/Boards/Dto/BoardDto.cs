using System.ComponentModel.DataAnnotations;

namespace Vtodo.UseCases.Handlers.Boards.Dto
{
    public class BoardDto
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public int PrioritySort { get; set; }
        
        [Required]
        public int ProjectId { get; set; }

        public string? ImageHeaderPath { get; set; }
    }
}