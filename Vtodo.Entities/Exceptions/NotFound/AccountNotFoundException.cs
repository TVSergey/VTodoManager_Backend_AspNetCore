using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class AccountNotFoundException : CustomException
    {
        public AccountNotFoundException(string message = "Account not found") : base(HttpStatusCode.NotFound, message)
        {
        }
    }
}