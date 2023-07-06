using System.Collections.Generic;
using MediatR;
using Vtodo.UseCases.Handlers.Boards.Dto;

namespace Vtodo.UseCases.Handlers.Boards.Queries.GetBoardsByProject
{
    public class GetBoardsByProjectRequest : IRequest<List<BoardDto>>
    {
        public int ProjectId { get; set; }
    }
}