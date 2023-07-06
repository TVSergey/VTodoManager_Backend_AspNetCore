using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class AttemptAddMemberFromGrantRoleException : CustomException
    {
        public AttemptAddMemberFromGrantRoleException (string message = "") : base(HttpStatusCode.InternalServerError, message)
        {
            
        }
    }
}