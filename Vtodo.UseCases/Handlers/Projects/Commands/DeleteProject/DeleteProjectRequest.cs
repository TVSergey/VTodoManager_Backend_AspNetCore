using MediatR;
using Vtodo.UseCases.Handlers.Projects.Dto;

namespace Vtodo.UseCases.Handlers.Projects.Commands.DeleteProject
{
    public class DeleteProjectRequest : IRequest
    {
        public int Id { get; set; }
    }
}