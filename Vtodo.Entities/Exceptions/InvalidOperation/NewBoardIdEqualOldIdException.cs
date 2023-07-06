using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class NewBoardIdEqualOldIdException : CustomException
    {
        public NewBoardIdEqualOldIdException(string message = "New board id should not be equal to old board id") : base(HttpStatusCode.BadRequest, message)
        {
        }
    }
}