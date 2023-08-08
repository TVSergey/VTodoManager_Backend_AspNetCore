using System;
using System.ComponentModel.DataAnnotations;
using Vtodo.Entities.Enums;

namespace Vtodo.UseCases.Handlers.Tasks.Dto
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public int BoardId { get; set; }
        
        public int? ParentTaskId { get; set; }
        
        [Required]
        [EnumDataType(typeof(TaskPriority))]
        public TaskPriority Priority { get; set; }
        
        [Required]
        public int PrioritySort { get; set; }
    }
}