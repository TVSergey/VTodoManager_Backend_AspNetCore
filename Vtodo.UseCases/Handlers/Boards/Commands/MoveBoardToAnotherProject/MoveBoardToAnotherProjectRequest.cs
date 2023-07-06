using MediatR;
using Vtodo.UseCases.Handlers.Boards.Dto;

namespace Vtodo.UseCases.Handlers.Boards.Commands.MoveBoardToAnotherProject
{
    public class MoveBoardToAnotherProjectRequest : IRequest
    {
        public int BoardId { get; set; }
        
        public int ProjectId { get; set; }
    }
}