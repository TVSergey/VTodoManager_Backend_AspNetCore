using MediatR;
using Vtodo.UseCases.Handlers.Boards.Dto;

namespace Vtodo.UseCases.Handlers.Boards.Commands.UpdateBoard
{
    public class UpdateBoardRequest : IRequest
    {
        public int Id { get; set; }
        public UpdateBoardDto UpdateBoardDto { get; set; } = null!;
    }
}