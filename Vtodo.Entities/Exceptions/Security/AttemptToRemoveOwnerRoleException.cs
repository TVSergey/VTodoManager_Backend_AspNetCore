using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class AttemptToRemoveOwnerRoleException : CustomException
    {
        public AttemptToRemoveOwnerRoleException(string message = "Attempt to revoke the Owner role") : base(HttpStatusCode.InternalServerError, message)
        {
            
        }
    }
}