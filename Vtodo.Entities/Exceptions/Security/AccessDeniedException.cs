using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class AccessDeniedException : CustomException
    {
        public AccessDeniedException(string message = "Access Denied") : base(HttpStatusCode.Forbidden, message)
        {
            
        }
    }
}