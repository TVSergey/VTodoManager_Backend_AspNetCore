using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vtodo.Entities.Models
{
    public class Board
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        
        public int PrioritySort { get; set; }
        
        [Required]
        public Project Project { get; set; } = null!;

        public ICollection<TaskM> Tasks { get; set; } = default!;
        
        public string? ImageHeaderPath { get; set; }
    }
}