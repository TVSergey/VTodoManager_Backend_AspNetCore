using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vtodo.Entities.Models
{
    public class Project
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;

        public ICollection<Board> Boards { get; set; } = default!;
        
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
    }
}