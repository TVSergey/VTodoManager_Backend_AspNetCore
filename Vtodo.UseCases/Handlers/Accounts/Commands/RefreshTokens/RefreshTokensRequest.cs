using MediatR;
using Vtodo.UseCases.Handlers.Accounts.Dto;

namespace Vtodo.UseCases.Handlers.Accounts.Commands.RefreshTokens
{
    public class RefreshTokensRequest : IRequest<JwtTokensDto>
    {
        public RefreshTokensDto RefreshTokensDto { get; set; } = null!;
    }
}