using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class TaskNotFoundException : CustomException
    {
        public TaskNotFoundException(string message = "Task not found") : base(HttpStatusCode.NotFound, message)
        {
            
        }
        
    }
}