using MediatR;
using Vtodo.UseCases.Handlers.Boards.Dto;

namespace Vtodo.UseCases.Handlers.Boards.Commands.DeleteBoard
{
    public class DeleteBoardRequest : IRequest
    {
        public int Id { get; set; }
    }
}