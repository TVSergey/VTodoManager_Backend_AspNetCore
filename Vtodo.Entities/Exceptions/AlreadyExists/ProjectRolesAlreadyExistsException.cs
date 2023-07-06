using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class ProjectRolesAlreadyExistsException : CustomException
    {
        public ProjectRolesAlreadyExistsException(string message = "Project role already exists") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}