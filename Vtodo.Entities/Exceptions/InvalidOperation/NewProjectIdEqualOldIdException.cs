using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class NewProjectIdEqualOldIdException : CustomException
    {
        public NewProjectIdEqualOldIdException(string message = "New project id should not be equal to old project id") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}