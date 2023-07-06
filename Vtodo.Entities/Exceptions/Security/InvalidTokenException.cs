using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class InvalidTokenException : CustomException
    {
        public InvalidTokenException(string message = "Invalid token") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}