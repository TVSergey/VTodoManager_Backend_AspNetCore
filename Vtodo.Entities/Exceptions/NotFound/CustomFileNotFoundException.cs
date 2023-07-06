using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class CustomFileNotFoundException : CustomException
    {
        public CustomFileNotFoundException(string message = "File not found") : base(HttpStatusCode.InternalServerError, message)
        {
        }
    }
}