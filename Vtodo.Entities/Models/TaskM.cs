using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vtodo.Entities.Enums;

namespace Vtodo.Entities.Models
{
    public class TaskM
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public int PrioritySort { get; set; }
        
        [Required]
        public TaskPriority Priority { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public bool IsCompleted { get; set; }

        [Required]
        public Board Board { get; set; } = null!;
        
        public TaskM? ParentTask { get; set; }

        public ICollection<TaskM> ChildrenTasks { get; set; } = default!;
        
        public string? ImageHeaderPath { get; set; }
    }
}