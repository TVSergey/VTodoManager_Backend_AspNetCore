using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class NewParentTaskIdEqualOldIdException : CustomException
    {
        public NewParentTaskIdEqualOldIdException(string message = "New parent task id should not be equal to old parent task id") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}