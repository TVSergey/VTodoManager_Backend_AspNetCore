using MediatR;
using Vtodo.UseCases.Handlers.Projects.Dto;

namespace Vtodo.UseCases.Handlers.Projects.Queries.GetProject
{
    public class GetProjectRequest : IRequest<ProjectDto>
    {
        public int Id { get; set; }
    }
}