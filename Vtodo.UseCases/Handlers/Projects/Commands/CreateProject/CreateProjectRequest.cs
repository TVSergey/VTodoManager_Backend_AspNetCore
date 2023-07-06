using MediatR;
using Vtodo.UseCases.Handlers.Projects.Dto;

namespace Vtodo.UseCases.Handlers.Projects.Commands.CreateProject
{
    public class CreateProjectRequest : IRequest
    {
        public CreateProjectDto CreateProjectDto { get; set; } = null!;
    }
}