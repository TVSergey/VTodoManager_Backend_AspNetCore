using System;
using System.IO;
using MediatR;
using Vtodo.UseCases.Handlers.Projects.Dto;

namespace Vtodo.UseCases.Handlers.Boards.Commands.UploadBoardHeaderBackground
{
    public class UploadBoardHeaderBackgroundRequest : IRequest
    {
        public int Id { get; set; }
        
        public Stream BackgroundImage { get; set; } = null!;
        
        public string FileName { get; set; } = String.Empty;
    }
}