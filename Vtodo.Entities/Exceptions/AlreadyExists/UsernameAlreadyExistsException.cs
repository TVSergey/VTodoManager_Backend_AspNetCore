using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class UsernameAlreadyExistsException : CustomException
    {
        public UsernameAlreadyExistsException(string message = "Username already taken") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}