using MediatR;
using Vtodo.UseCases.Handlers.ProjectsRoles.Dto;

namespace Vtodo.UseCases.Handlers.ProjectsRoles.Commands.RevokeRole
{
    public class RevokeRoleRequest : IRequest
    {
        public int ProjectId { get; set; }
        
        public RevokeRoleDto RevokeRoleDto { get; set; } = null!;
    }
}