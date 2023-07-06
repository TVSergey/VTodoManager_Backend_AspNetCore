using System.Net;

namespace Vtodo.Entities.Exceptions
{
    public class AccountNotMemberInProjectException : CustomException
    {
        public AccountNotMemberInProjectException(string message = "Account is not a member in project") : base(HttpStatusCode.BadRequest, message)
        {

        }
    }
}