using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class ExpiredTokenException : CustomException
    {
        public ExpiredTokenException(string message = "Expired token") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}