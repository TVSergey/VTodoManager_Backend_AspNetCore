using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class InvalidPasswordException : CustomException  
    {
        public InvalidPasswordException(string message = "Invalid password") : base(HttpStatusCode.Forbidden, message)
        {
        
        }
    }
}