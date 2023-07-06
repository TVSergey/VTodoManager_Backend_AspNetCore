using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class ProjectRoleNotFoundException : CustomException
    {
        public ProjectRoleNotFoundException(string message = "Project role not found") : base(HttpStatusCode.NotFound, message)
        {
            
        }
    }
}