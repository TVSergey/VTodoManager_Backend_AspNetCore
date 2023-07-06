using System.ComponentModel.DataAnnotations;
using Vtodo.Entities.Enums;

namespace Vtodo.UseCases.Handlers.ProjectsRoles.Dto
{
    public class GrantRoleDto
    {
        //TODO переделать на username / email
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        [EnumDataType(typeof(ProjectRoles))]
        public ProjectRoles Role { get; set; }
    }
}