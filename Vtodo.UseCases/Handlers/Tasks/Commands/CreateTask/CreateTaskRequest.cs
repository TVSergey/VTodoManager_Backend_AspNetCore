using MediatR;
using Vtodo.UseCases.Handlers.Tasks.Dto;

namespace Vtodo.UseCases.Handlers.Tasks.Commands.CreateTask
{
    public class CreateTaskRequest : IRequest
    {
        public CreateTaskDto CreateTaskDto { get; set; } = null!;
    }
}