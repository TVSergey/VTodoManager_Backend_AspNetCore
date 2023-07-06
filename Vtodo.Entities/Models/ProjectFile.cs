using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vtodo.Entities.Models
{
    public class ProjectFile
    {
        [ForeignKey("Project"), Column(Order = 0)]
        public int ProjectId { get; set; }
        
        [Required]
        public string FileName { get; set; } = string.Empty;
    }
}