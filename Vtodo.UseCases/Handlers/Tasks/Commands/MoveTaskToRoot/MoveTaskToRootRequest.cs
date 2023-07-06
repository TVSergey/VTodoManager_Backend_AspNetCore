using MediatR;
using Vtodo.UseCases.Handlers.Tasks.Dto;

namespace Vtodo.UseCases.Handlers.Tasks.Commands.MoveTaskToRoot
{
    public class MoveTaskToRootRequest : IRequest
    {
        public int Id { get; set; }
    }
}