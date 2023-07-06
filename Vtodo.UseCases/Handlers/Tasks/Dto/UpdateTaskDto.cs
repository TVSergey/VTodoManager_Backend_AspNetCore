using System.ComponentModel.DataAnnotations;
using Vtodo.Entities.Enums;

namespace Vtodo.UseCases.Handlers.Tasks.Dto
{
    public class UpdateTaskDto
    {
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
        
        public int? EndDateTimeStamp { get; set; }
        
        [Required]
        [EnumDataType(typeof(TaskPriority))]
        public TaskPriority Priority { get; set; }
        
        public int PrioritySort { get; set; }
        
    }
}