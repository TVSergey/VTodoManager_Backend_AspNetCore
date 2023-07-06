using System;
using System.IO;
using MediatR;
using Vtodo.UseCases.Handlers.Projects.Dto;

namespace Vtodo.UseCases.Handlers.Boards.Commands.DeleteBoardHeaderBackground
{
    public class DeleteBoardHeaderBackgroundRequest : IRequest
    {
        public int Id { get; set; }
    }
}