using System.ComponentModel.DataAnnotations;
using Vtodo.Entities.Enums;

namespace Vtodo.UseCases.Handlers.ProjectsRoles.Dto
{
    public class ChangeOwnerDto
    {
        //TODO переделать на username / email
        [Required]
        public int AccountId { get; set; }
    }
}