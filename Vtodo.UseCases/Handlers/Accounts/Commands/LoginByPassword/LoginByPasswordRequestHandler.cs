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

namespace Vtodo.UseCases.Handlers.Accounts.Commands.LoginByPassword
{
    internal class LoginByPasswordRequestHandler : IRequestHandler<LoginByPasswordRequest, JwtTokensDto>
    {
        private readonly IDbContext _dbContext;
        private readonly ISecurityService _securityService;
        private readonly IJwtService _jwtService;
        private readonly IConfigService _configService;

        public LoginByPasswordRequestHandler(
            IDbContext dbContext, 
            ISecurityService securityService,
            IJwtService jwtService,
            IConfigService configService)
        {
            _dbContext = dbContext;
            _securityService = securityService;
            _jwtService = jwtService;
            _configService = configService;
        }
        
        public async Task<JwtTokensDto> Handle(LoginByPasswordRequest request, CancellationToken cancellationToken)
        {
            var loginDto = request.LoginByPasswordDto;
            
            var account = _dbContext.Accounts.FirstOrDefault(x => x.Email == loginDto.Email);
            if (account == null) throw new AccountNotFoundException();
            
            var isRightPassword = _securityService.VerifyPassword(loginDto.Password, account.HashedPassword, _configService.HasherKeySize,
                _configService.HasherIterations, account.Salt);
            
            if (!isRightPassword) throw new InvalidPasswordException();
            
            var accessToken = _jwtService.GenerateNewTokensAfterLogin(account, out string refreshToken);
            
            return new JwtTokensDto() {AccessToken = accessToken, RefreshToken = refreshToken};
        }
    }
}