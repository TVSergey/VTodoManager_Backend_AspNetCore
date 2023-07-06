using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class BoardNotFoundException : CustomException
    {
        public BoardNotFoundException(string message = "Board not found") : base(HttpStatusCode.NotFound, message)
        {
        }
    }
}