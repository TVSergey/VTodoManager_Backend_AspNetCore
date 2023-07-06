using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class EmailAlreadyExistsException : CustomException
    {
        public EmailAlreadyExistsException(string message = "Email already taken") : base(HttpStatusCode.BadRequest, message)
        {
        }
    }
}