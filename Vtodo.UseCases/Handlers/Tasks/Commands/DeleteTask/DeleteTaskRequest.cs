using MediatR;
using Vtodo.UseCases.Handlers.Tasks.Dto;

namespace Vtodo.UseCases.Handlers.Tasks.Commands.DeleteTask
{
    public class DeleteTaskRequest : IRequest
    {
        public int Id { get; set; }
    }
}