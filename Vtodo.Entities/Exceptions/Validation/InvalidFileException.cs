using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class InvalidFileException : CustomException
    {
        public InvalidFileException(string message = "Invalid file") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}