using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class UnauthorizedException : CustomException
    {
        public UnauthorizedException(string message = "Unauthorized") : base(HttpStatusCode.Unauthorized, message)
        {
            
        }
    }
}