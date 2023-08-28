using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class AttemptChangeOwnerFromGrantRoleException : CustomException
    {
        public AttemptChangeOwnerFromGrantRoleException (string message = "Using a 'grant role' instead of a 'change owner' to change owner") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}