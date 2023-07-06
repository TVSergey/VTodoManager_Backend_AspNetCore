using MediatR;
using Vtodo.UseCases.Handlers.Accounts.Dto;

namespace Vtodo.UseCases.Handlers.Accounts.Commands.CreateAccount
{
    public class CreateAccountRequest : IRequest<JwtTokensDto>
    {
        public CreateAccountDto CreateAccountDto { get; set; } = null!;
    }
}