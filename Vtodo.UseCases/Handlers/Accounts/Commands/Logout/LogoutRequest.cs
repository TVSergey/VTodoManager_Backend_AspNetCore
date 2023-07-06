using MediatR;
using Vtodo.UseCases.Handlers.Accounts.Dto;

namespace Vtodo.UseCases.Handlers.Accounts.Commands.Logout
{
    public class LogoutRequest : IRequest
    {
        public LogoutDto LogoutDto { get; set; } = null!;
    }
}