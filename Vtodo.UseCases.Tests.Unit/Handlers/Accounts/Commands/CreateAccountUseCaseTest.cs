using System.Linq;
using System.Threading;
using AutoMapper;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.DomainServices.Interfaces;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Vtodo.UseCases.Handlers.Accounts.Commands.CreateAccount;
using Vtodo.UseCases.Handlers.Accounts.Dto;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Accounts.Commands
{
    public class CreateAccountRequestHandlerTest
    {
        
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulCreateAccount_ReturnsTaskJwtTokensDto()
        {
            SetupDbContext();

            var createAccountDto = new CreateAccountDto()
                { Email = "test@test.ru", Username = "test", Password = "test", ConfirmPassword = "test"};
            
            var request = new CreateAccountRequest() { CreateAccountDto = createAccountDto };
            
            var mapperMock = SetupMockMapper();
            mapperMock.Setup(x => x.Map<Account>(It.IsAny<CreateAccountDto>()))
                .Returns(new Account() {Email = createAccountDto.Email, Username = createAccountDto.Username});
            
            var createAccountRequestHandler = new CreateAccountRequestHandler(
                _dbContext,
                SetupMockSecurityService().Object,
                SetupMockJwtService().Object,
                SetupMockConfigService().Object,
                mapperMock.Object
            );
            
            var result = createAccountRequestHandler.Handle(request, CancellationToken.None);
            
            Assert.NotNull(_dbContext.Accounts.FirstOrDefault(x => 
                x.Email == createAccountDto.Email && 
                x.Username == createAccountDto.Username));
            
            Assert.NotNull(result);
            
            CleanUp();
        }

        [Fact]
        public async void Handle_EmailAlreadyExists_ThrowsEmailAlreadyExistsException()
        {
            SetupDbContext();

            var createAccountDto = new CreateAccountDto()
                { Email = "test@test.ru", Username = "test", Password = "test", ConfirmPassword = "test"};

            _dbContext.Accounts.Add(new Account() {Email = createAccountDto.Email, Username = createAccountDto.Username, HashedPassword = "test", Salt = new byte[64]});
            _dbContext.SaveChanges();
            
            var request = new CreateAccountRequest() { CreateAccountDto = createAccountDto };
            
            var mapperMock = SetupMockMapper();
            mapperMock.Setup(x => x.Map<Account>(It.IsAny<CreateAccountDto>()))
                .Returns(new Account() {Email = createAccountDto.Email, Username = createAccountDto.Username});
            
            var createAccountRequestHandler = new CreateAccountRequestHandler(
                _dbContext,
                SetupMockSecurityService().Object,
                SetupMockJwtService().Object,
                SetupMockConfigService().Object,
                mapperMock.Object
            );
            
            await Assert.ThrowsAsync<EmailAlreadyExistsException>(() => createAccountRequestHandler.Handle(request, CancellationToken.None));
            
            CleanUp();
        }
        
        [Fact]
        public async void Handle_UsernameAlreadyExists_ThrowsUsernameAlreadyExistsException()
        {
            SetupDbContext();

            var createAccountDto = new CreateAccountDto()
                { Email = "test@test.ru", Username = "test", Password = "test", ConfirmPassword = "test"};

            _dbContext.Accounts.Add(new Account() {Email = "test", Username = createAccountDto.Username, HashedPassword = "test", Salt = new byte[64]});
            _dbContext.SaveChanges();
            
            var request = new CreateAccountRequest() { CreateAccountDto = createAccountDto };
            
            var mapperMock = SetupMockMapper();
            mapperMock.Setup(x => x.Map<Account>(It.IsAny<CreateAccountDto>()))
                .Returns(new Account() {Email = createAccountDto.Email, Username = createAccountDto.Username});
            
            var createAccountRequestHandler = new CreateAccountRequestHandler(
                _dbContext,
                SetupMockSecurityService().Object,
                SetupMockJwtService().Object,
                SetupMockConfigService().Object,
                mapperMock.Object
            );
            
            await Assert.ThrowsAsync<UsernameAlreadyExistsException>(() => createAccountRequestHandler.Handle(request, CancellationToken.None));
            
            CleanUp();
        }
        
        [Fact]
        public async void Handle_PasswordsNotEquals_ThrowsPasswordsNotEqualsException()
        {
            SetupDbContext();

            var createAccountDto = new CreateAccountDto()
                { Email = "test@test.ru", Username = "test", Password = "test", ConfirmPassword = "test2"};

            var request = new CreateAccountRequest() { CreateAccountDto = createAccountDto };
            
            var mapperMock = SetupMockMapper();
            mapperMock.Setup(x => x.Map<Account>(It.IsAny<CreateAccountDto>()))
                .Returns(new Account() {Email = createAccountDto.Email, Username = createAccountDto.Username});
            
            var createAccountRequestHandler = new CreateAccountRequestHandler(
                _dbContext,
                SetupMockSecurityService().Object,
                SetupMockJwtService().Object,
                SetupMockConfigService().Object,
                mapperMock.Object
            );
            
            await Assert.ThrowsAsync<PasswordsNotEqualsException>(() => createAccountRequestHandler.Handle(request, CancellationToken.None));
            
            CleanUp();
        }
        
        private static Mock<IMapper> SetupMockMapper()
        {
            return new Mock<IMapper>();
        }
        
        private static Mock<ISecurityService> SetupMockSecurityService()
        {
            var mock = new Mock<ISecurityService>();

            var salt = new byte[SetupMockConfigService().Object.HasherKeySize];
            
            mock.Setup(x =>
                x.HashPassword(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), out salt)).Returns("testtesttest");
            
            return mock;
        }
        
        private static Mock<IJwtService> SetupMockJwtService()
        {
            var mock = new Mock<IJwtService>();
            
            string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJ1c2VybmFtZSI6InRlc3QiLCJleHAiOi0xLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDozMDAwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6MzAwMCJ9.dmwgxpp5XoLtGSgfsJVPLbm_8JPJa6gSmgPCyUoHm2k";
            string refreshToken =  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJ1c2VybmFtZSI6InRlc3QiLCJleHAiOi0xLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDozMDAwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6MzAwMCJ9.dmwgxpp5XoLtGSgfsJVPLbm_8JPJa6gSmgPCyUoHm2k";

            mock.Setup(x => x.GenerateNewTokensAfterLogin(It.IsAny<Account>(), out refreshToken))
                .Returns(accessToken);
            
            return mock;
        }
        
        private static Mock<IConfigService> SetupMockConfigService()
        {
            var mock = new Mock<IConfigService>();
            mock.Setup(x => x.HasherKeySize).Returns(64);
            mock.Setup(x => x.HasherIterations).Returns(400000);
            
            return mock;
        }

        private void SetupDbContext()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();
        }
        
        private void CleanUp()
        {
            _dbContext?.Database.EnsureDeleted();
            _dbContext?.Dispose();
        }
    }
}