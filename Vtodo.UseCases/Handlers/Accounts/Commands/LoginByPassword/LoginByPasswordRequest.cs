using MediatR;
using Vtodo.UseCases.Handlers.Accounts.Dto;

namespace Vtodo.UseCases.Handlers.Accounts.Commands.LoginByPassword
{
    public class LoginByPasswordRequest : IRequest<JwtTokensDto>
    {
        public LoginByPasswordDto LoginByPasswordDto { get; set; } = null!;
    }
}