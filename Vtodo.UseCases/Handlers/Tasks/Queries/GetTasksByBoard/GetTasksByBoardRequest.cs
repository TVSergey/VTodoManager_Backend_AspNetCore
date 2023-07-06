using System.Collections.Generic;
using MediatR;
using Vtodo.UseCases.Handlers.Tasks.Dto;

namespace Vtodo.UseCases.Handlers.Tasks.Queries.GetTasksByBoard
{
    public class GetTasksByBoardRequest : IRequest<List<TaskDto>>
    {
        public int BoardId { get; set; }
    }
}