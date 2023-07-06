namespace Vtodo.UseCases.Handlers.Tasks.Dto
{
    public class TaskDto
    {
        public int Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public long EndDate { get; set; }
        
        public bool IsCompleted { get; set; }
        
        public int BoardId { get; set; }
        public int? ParentId { get; set; }
        
        public int PrioritySort { get; set; }
        
        public int Priority { get; set; }
        
        public string? ImageHeaderPath { get; set; }
    }
}