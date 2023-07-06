using MediatR;
using Vtodo.UseCases.Handlers.ProjectsRoles.Dto;

namespace Vtodo.UseCases.Handlers.ProjectsRoles.Commands.RevokeAllRole
{
    public class KickMemberRequest : IRequest
    {
        public int ProjectId { get; set; }
        
        public KickMemberDto KickMemberDto { get; set; } = null!;
    }
}