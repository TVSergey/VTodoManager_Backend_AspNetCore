using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class ProjectNotFoundException : CustomException
    {
        public ProjectNotFoundException(string message = "Project not found") : base(HttpStatusCode.NotFound, message)
        {
            
        }
    }
}