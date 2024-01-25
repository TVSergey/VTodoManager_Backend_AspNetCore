using System.IO;
using MediatR;

namespace Vtodo.UseCases.Handlers.Boards.Queries.GetBoardHeaderBackground
{
    public class GetBoardHeaderBackgroundRequest : IRequest<FileStream?>
    {
        public int Id { get; set; }
    }
}