using MediatR;
using Vtodo.UseCases.Handlers.Boards.Dto;

namespace Vtodo.UseCases.Handlers.Boards.Queries.GetBoard
{
    public class GetBoardRequest : IRequest<BoardDto>
    {
        public int Id { get; set; }
    }
}