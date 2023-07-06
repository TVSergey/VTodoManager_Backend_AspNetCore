using MediatR;
using Vtodo.UseCases.Handlers.Tasks.Dto;

namespace Vtodo.UseCases.Handlers.Tasks.Commands.UpdateTask
{
    public class UpdateTaskRequest : IRequest
    {
        public int Id { get; set; }
        public UpdateTaskDto UpdateTaskDto { get; set; } = null!;
    }
}