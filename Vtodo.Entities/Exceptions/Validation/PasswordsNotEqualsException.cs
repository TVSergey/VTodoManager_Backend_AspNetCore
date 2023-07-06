using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class PasswordsNotEqualsException : CustomException
    {
        public PasswordsNotEqualsException(string message = "Passwords not equals") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}