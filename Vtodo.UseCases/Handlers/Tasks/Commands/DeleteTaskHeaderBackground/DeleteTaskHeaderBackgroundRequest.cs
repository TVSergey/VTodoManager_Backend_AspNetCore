using System;
using System.IO;
using MediatR;

namespace Vtodo.UseCases.Handlers.Tasks.Commands.DeleteTaskHeaderBackground
{
    public class DeleteTaskHeaderBackgroundRequest : IRequest
    {
        public int Id { get; set; }
    }
}