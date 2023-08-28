using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class AttemptAddMemberFromGrantRoleException : CustomException
    {
        public AttemptAddMemberFromGrantRoleException (string message = "Using a 'grant role' instead of a 'add member' to add a user") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}