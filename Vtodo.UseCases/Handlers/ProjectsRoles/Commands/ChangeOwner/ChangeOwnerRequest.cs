using MediatR;
using Vtodo.UseCases.Handlers.ProjectsRoles.Dto;

namespace Vtodo.UseCases.Handlers.ProjectsRoles.Commands.ChangeOwner
{
    public class ChangeOwnerRequest : IRequest
    {
        public int ProjectId { get; set; }
        
        public ChangeOwnerDto ChangeOwnerDto { get; set; } = null!;
    }
}