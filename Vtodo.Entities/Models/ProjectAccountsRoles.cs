using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vtodo.Entities.Enums;

namespace Vtodo.Entities.Models
{
    public class ProjectAccountsRoles
    {
        [ForeignKey("Project"), Column(Order = 0)]
        public int ProjectId { get; set; }
        
        [ForeignKey("Account"), Column(Order = 1)]
        public int AccountId { get; set; }
        
        public Project Project { get; set; } = null!;
        
        public Account Account { get; set; } = null!;
        
        [Column(Order = 2)]
        public ProjectRoles ProjectRole { get; set; }
    }
}