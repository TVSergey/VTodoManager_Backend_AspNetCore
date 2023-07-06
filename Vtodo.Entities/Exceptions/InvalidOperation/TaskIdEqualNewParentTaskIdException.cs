using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class TaskIdEqualNewParentTaskIdException : CustomException
    {
        public TaskIdEqualNewParentTaskIdException(string message = "New parent task id should not be equal to task id") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}