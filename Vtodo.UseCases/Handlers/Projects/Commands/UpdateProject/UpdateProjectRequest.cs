using MediatR;
using Vtodo.UseCases.Handlers.Projects.Dto;

namespace Vtodo.UseCases.Handlers.Projects.Commands.UpdateProject
{
    public class UpdateProjectRequest : IRequest
    {
        public int Id { get; set; }
        public UpdateProjectDto UpdateProjectDto { get; set; } = null!;
    }
}