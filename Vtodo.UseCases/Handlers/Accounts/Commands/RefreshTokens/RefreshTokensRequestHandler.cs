using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Vtodo.DomainServices.Interfaces;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.UseCases.Handlers.Accounts.Dto;

namespace Vtodo.UseCases.Handlers.Accounts.Commands.RefreshTokens
{
    internal class RefreshTokensRequestHandler : IRequestHandler<RefreshTokensRequest, JwtTokensDto>
    {
        private readonly IDbContext _dbContext;
        private readonly IJwtService _jwtService;

        public RefreshTokensRequestHandler(
            IDbContext dbContext,
            IJwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;

        }
        
        public async Task<JwtTokensDto> Handle(RefreshTokensRequest request, CancellationToken cancellationToken)
        {
            var accessToken = _jwtService.RefreshTokens(request.RefreshTokensDto.RefreshToken, out string refreshToken);
            
            return new JwtTokensDto() {AccessToken = accessToken, RefreshToken = refreshToken};
        }
    }
}