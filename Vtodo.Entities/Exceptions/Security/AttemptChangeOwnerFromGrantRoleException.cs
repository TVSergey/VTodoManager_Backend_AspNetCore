using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class AttemptChangeOwnerFromGrantRoleException : CustomException
    {
        public AttemptChangeOwnerFromGrantRoleException (string message = "") : base(HttpStatusCode.InternalServerError, message)
        {
            
        }
    }
}