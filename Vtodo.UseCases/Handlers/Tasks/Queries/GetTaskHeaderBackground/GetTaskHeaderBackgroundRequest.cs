using System.IO;
using MediatR;

namespace Vtodo.UseCases.Handlers.Tasks.Queries.GetTaskHeaderBackground
{
    public class GetTaskHeaderBackgroundRequest : IRequest<FileStream?>
    {
        public int Id { get; set; }
    }
}