using System;
using System.IO;
using MediatR;

namespace Vtodo.UseCases.Handlers.Tasks.Commands.UploadTaskHeaderBackground
{
    public class UploadTaskHeaderBackgroundRequest : IRequest
    {
        public int Id { get; set; }
        
        public Stream BackgroundImage { get; set; } = null!;
        
        public string FileName { get; set; } = String.Empty;
    }
}