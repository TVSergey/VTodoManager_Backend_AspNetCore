using System;
using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public abstract class CustomException : Exception
    {
        private CustomException() { }
        
        protected CustomException(HttpStatusCode code)
        {
            Code = code;
        }
        
        protected CustomException(HttpStatusCode code, string message) : base(message)
        {
            Code = code;
        }
        
        public HttpStatusCode Code { get; set; }
    }
}