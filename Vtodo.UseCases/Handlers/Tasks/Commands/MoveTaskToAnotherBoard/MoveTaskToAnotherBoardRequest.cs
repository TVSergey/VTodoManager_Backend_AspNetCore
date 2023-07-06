using MediatR;

namespace Vtodo.UseCases.Handlers.Tasks.Commands.MoveTaskToAnotherBoard
{
    public class MoveTaskToAnotherBoardRequest : IRequest
    {
        public int TaskId { get; set; }
        public int NewBoardId { get; set; }
    }
}