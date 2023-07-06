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

namespace Vtodo.UseCases.Handlers.Accounts.Commands.CreateAccount
{
    internal class CreateAccountRequestHandler : IRequestHandler<CreateAccountRequest, JwtTokensDto>
    {
        private readonly IDbContext _dbContext;
        private readonly ISecurityService _securityService;
        private readonly IJwtService _jwtService;
        private readonly IConfigService _configService;
        private readonly IMapper _mapper;
        
        public CreateAccountRequestHandler(
            IDbContext dbContext, 
            ISecurityService securityService,
            IJwtService jwtService,
            IConfigService configService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _securityService = securityService;
            _jwtService = jwtService;
            _configService = configService;
            _mapper = mapper;
        }
        
        public async Task<JwtTokensDto> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var createDto = request.CreateAccountDto;
            var account = _mapper.Map<Account>(createDto);
            
            if (_dbContext.Accounts.FirstOrDefault(x => x.Email == createDto.Email) != null) throw new EmailAlreadyExistsException();
            
            if (_dbContext.Accounts.FirstOrDefault(x => x.Username == createDto.Username) != null) throw new UsernameAlreadyExistsException();

            if (createDto.Password != createDto.ConfirmPassword)
                throw new PasswordsNotEqualsException();
            
            if (!string.IsNullOrWhiteSpace(createDto.Firstname)) account.Firstname = createDto.Firstname;
            if (!string.IsNullOrWhiteSpace(createDto.Surname)) account.Surname = createDto.Surname;

            account.HashedPassword = _securityService.HashPassword(createDto.Password, _configService.HasherKeySize, _configService.HasherIterations, out byte[] salt);
            account.Salt = salt;
            
            _dbContext.Accounts.Add(account);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var accessToken = _jwtService.GenerateNewTokensAfterLogin(account, out string refreshToken);
            
            return new JwtTokensDto() {AccessToken = accessToken, RefreshToken = refreshToken};
        }
    }
}