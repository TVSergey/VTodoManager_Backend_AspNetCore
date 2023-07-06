using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class AttemptGetNullFileException : CustomException
    {
        public AttemptGetNullFileException(string message = "File not specified") : base(HttpStatusCode.BadRequest, message)
        {
            
        }
    }
}