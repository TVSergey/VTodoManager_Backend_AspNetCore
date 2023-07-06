using MediatR;

namespace Vtodo.UseCases.Handlers.Tasks.Commands.MoveTaskToAnotherTask
{
    public class MoveTaskToAnotherTaskRequest : IRequest
    {
        public int TaskId { get; set; }
        public int NewParentTaskId { get; set; }
    }
}