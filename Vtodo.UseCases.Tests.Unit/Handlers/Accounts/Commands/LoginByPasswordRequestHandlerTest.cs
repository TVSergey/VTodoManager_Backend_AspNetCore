using System.Threading;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.DomainServices.Interfaces;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Vtodo.UseCases.Handlers.Accounts.Commands.LoginByPassword;
using Vtodo.UseCases.Handlers.Accounts.Dto;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Accounts.Commands
{
    public class LoginByPasswordRequestHandlerTest
    {
        
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulLoginByPassword_ReturnsTaskJwtTokensDto()
        {
            SetupDbContext();
            
            var loginByPasswordDto = new LoginByPasswordDto()
                { Email = "test@test.ru", Password = "test"};
            
            var request = new LoginByPasswordRequest() { LoginByPasswordDto = loginByPasswordDto };
            
            _dbContext.Accounts.Add(new Account() {Email = loginByPasswordDto.Email, Username = "test", HashedPassword = "test", Salt = new byte[64]});
            _dbContext.SaveChanges();

            var mockSecurityService = SetupMockSecurityService();
            mockSecurityService.Setup(x => x.VerifyPassword(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<byte[]>())).Returns(true);
            
            var loginByPasswordRequestHandler = new LoginByPasswordRequestHandler(
                _dbContext,
                mockSecurityService.Object,
                SetupMockJwtService().Object,
                SetupMockConfigService().Object
            );

            var result = loginByPasswordRequestHandler.Handle(request, CancellationToken.None);
            
            Assert.NotNull(result);
            
            CleanUp();
        }

        [Fact]
        public async void Handle_AccountNotFound_ThrowsAccountNotFoundException()
        {
            SetupDbContext();
            
            var loginByPasswordDto = new LoginByPasswordDto()
                { Email = "test@test.ru", Password = "test"};
            
            var request = new LoginByPasswordRequest() { LoginByPasswordDto = loginByPasswordDto };

            var mockSecurityService = SetupMockSecurityService();
            mockSecurityService.Setup(x => x.VerifyPassword(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<byte[]>())).Returns(true);
            
            var loginByPasswordRequestHandler = new LoginByPasswordRequestHandler(
                _dbContext,
                mockSecurityService.Object,
                SetupMockJwtService().Object,
                SetupMockConfigService().Object
            );
            
            await Assert.ThrowsAsync<AccountNotFoundException>(() => loginByPasswordRequestHandler.Handle(request, CancellationToken.None));
            
            CleanUp();
        }
        
        [Fact]
        public async void Handle_InvalidPassword_ThrowsInvalidPasswordException()
        {
            SetupDbContext();
            
            var loginByPasswordDto = new LoginByPasswordDto()
                { Email = "test@test.ru", Password = "test"};
            
            var request = new LoginByPasswordRequest() { LoginByPasswordDto = loginByPasswordDto };

            _dbContext.Accounts.Add(new Account() {Email = loginByPasswordDto.Email, Username = "test", HashedPassword = "test", Salt = new byte[64]});
            _dbContext.SaveChanges();
            
            var mockSecurityService = SetupMockSecurityService();
            mockSecurityService.Setup(x => x.VerifyPassword(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<byte[]>())).Returns(false);
            
            var loginByPasswordRequestHandler = new LoginByPasswordRequestHandler(
                _dbContext,
                mockSecurityService.Object,
                SetupMockJwtService().Object,
                SetupMockConfigService().Object
            );
            
            await Assert.ThrowsAsync<InvalidPasswordException>(() => loginByPasswordRequestHandler.Handle(request, CancellationToken.None));
            
            CleanUp();
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