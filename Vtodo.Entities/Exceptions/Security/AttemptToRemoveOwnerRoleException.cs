using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class AttemptToRemoveOwnerRoleException : CustomException
    {
        public AttemptToRemoveOwnerRoleException(string message = "Using a 'remove role' instead of a 'change owner' to remove owner") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}