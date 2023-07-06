using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class AnotherBoardException : CustomException
    {
        public AnotherBoardException(string message = "Another board when moving task to task") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}