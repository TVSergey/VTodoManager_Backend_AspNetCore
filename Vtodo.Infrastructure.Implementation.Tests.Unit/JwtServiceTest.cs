using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Extensions.Options;
using Xunit;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Implementation.Options;
using Vtodo.Infrastructure.Implementation.Services;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;

namespace Vtodo.Infrastructure.Implementation.Tests.Unit
{
    public class JwtServiceTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void GenerateToken_OnSuccessfulAccessTokenGeneration_ReturnsString()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();
            var clientInfoService = new Mock<IClientInfoService>();
            
            _dbContext.Accounts.AddRange(GetTestAccountsList());
            _dbContext.SaveChanges();
            
            var jwtService = new JwtService(_dbContext, SetupJwtOptions().Object, clientInfoService.Object);

            var resultToken = jwtService.GenerateToken(_dbContext.Accounts.First());
            Assert.NotNull(resultToken);
            Assert.NotEmpty(resultToken);
            
            new JwtSecurityTokenHandler().ReadJwtToken(resultToken);
            CleanUp();
        }

        [Fact]
        public void GenerateRefreshToken_OnSuccessfulRefreshTokenGeneration_ReturnsString()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();
            var clientInfoService = new Mock<IClientInfoService>();
            
            _dbContext.Accounts.AddRange(GetTestAccountsList());
            _dbContext.SaveChanges();
            
            var jwtService = new JwtService(_dbContext, SetupJwtOptions().Object, clientInfoService.Object);

            var resultToken = jwtService.GenerateRefreshToken(_dbContext.Accounts.First(), "127.0.0.1", "Linux");
            Assert.NotNull(resultToken);
            Assert.NotEmpty(resultToken);
            
            new JwtSecurityTokenHandler().ReadJwtToken(resultToken);

            Assert.NotNull(_dbContext.RefreshTokens.FirstOrDefault(x => x.Token == resultToken));
            CleanUp();
        }

        [Fact]
        public void InvalidateRefreshToken_OnSuccessfulInvalidateToken_ReturnsVoid()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();
            var clientInfoService = new Mock<IClientInfoService>();
            
            _dbContext.Accounts.AddRange(GetTestAccountsList());
            _dbContext.SaveChanges();
            
            var jwtService = new JwtService(_dbContext, SetupJwtOptions().Object, clientInfoService.Object);
            
            var token = jwtService.GenerateRefreshToken(_dbContext.Accounts.First(), "127.0.0.1", "Linux");

            jwtService.InvalidateRefreshToken(_dbContext.Accounts.First(), token); 
            Assert.Empty(_dbContext.RefreshTokens);
            CleanUp();
        }
        
        [Fact]
        public void InvalidateRefreshToken_OnAccountNotOwnerToken_ThrowsAccessDeniedException()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();
            var clientInfoService = new Mock<IClientInfoService>();
            
            _dbContext.Accounts.AddRange(GetTestAccountsList());
            _dbContext.SaveChanges();
            
            var jwtService = new JwtService(_dbContext, SetupJwtOptions().Object, clientInfoService.Object);
            
            var token = jwtService.GenerateRefreshToken(_dbContext.Accounts.First(), "127.0.0.1", "Linux");

            Assert.Throws<AccessDeniedException>(() => jwtService.InvalidateRefreshToken(_dbContext.Accounts.First(x => x.Id == 2), token)); 
            CleanUp();
        }

        [Fact]
        public void InvalidateRefreshToken_OnExpiredToken_ThrowsExpiredTokenException()
        {
            var dateTimeToTest = DateTime.MinValue;
            
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();
            var clientInfoService = new Mock<IClientInfoService>();
            
            _dbContext.Accounts.AddRange(GetTestAccountsList());
            _dbContext.SaveChanges();
            
            var jwtService = new JwtService(_dbContext, SetupJwtOptions().Object, clientInfoService.Object);

            var token = jwtService.GenerateRefreshToken(_dbContext.Accounts.First(), "127.0.0.1", "Linux");
            
            var tokenFromDb = _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == token);
            if (tokenFromDb != null) tokenFromDb.ExpireAt = dateTimeToTest;

            _dbContext.SaveChanges();
            
            Assert.Throws<ExpiredTokenException>(() => jwtService.InvalidateRefreshToken(_dbContext.Accounts.First(), token)); 
            CleanUp();
        }
        
        [Fact]
        public void InvalidateAllRefreshTokens_OnSuccessfulInvalidateAllTokens_ReturnsVoid()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();
            var clientInfoService = new Mock<IClientInfoService>();
            
            _dbContext.Accounts.AddRange(GetTestAccountsList());
            _dbContext.SaveChanges();
            
            var jwtService = new JwtService(_dbContext, SetupJwtOptions().Object, clientInfoService.Object);
            
            jwtService.GenerateRefreshToken(_dbContext.Accounts.First(), "127.0.0.1", "Linux");
            jwtService.GenerateRefreshToken(_dbContext.Accounts.First(), "127.0.0.2", "Linux");
            jwtService.GenerateRefreshToken(_dbContext.Accounts.First(), "127.0.0.3", "Linux");
            jwtService.GenerateRefreshToken(_dbContext.Accounts.First(), "127.0.0.4", "Linux");
            jwtService.GenerateRefreshToken(_dbContext.Accounts.First(), "127.0.0.5", "Linux");
            jwtService.GenerateRefreshToken(_dbContext.Accounts.First(), "127.0.0.6", "Linux");

            jwtService.InvalidateAllRefreshTokens(_dbContext.Accounts.First()); 
            Assert.Empty(_dbContext.RefreshTokens);
            CleanUp();
        }
        
        [Fact]
        public void GenerateNewTokensAfterLogin_OnSuccessfulTokensGeneration_ReturnsRefreshAndAccessTokenAsString()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();

            var clientInfoService = new Mock<IClientInfoService>();
            clientInfoService.Setup(x => x.Ip).Returns("127.0.0.1");
            clientInfoService.Setup(x => x.DeviceInfo).Returns("Linux");
            
            _dbContext.Accounts.AddRange(GetTestAccountsList());
            _dbContext.SaveChanges();
            
            var jwtService = new JwtService(_dbContext, SetupJwtOptions().Object, clientInfoService.Object);
            
            var resultAccessToken = jwtService.GenerateNewTokensAfterLogin(_dbContext.Accounts.First(), out string resultRefreshToken);
            Assert.NotNull(resultAccessToken);
            Assert.NotEmpty(resultAccessToken);
            
            Assert.NotNull(resultRefreshToken);
            Assert.NotEmpty(resultRefreshToken);

            new JwtSecurityTokenHandler().ReadJwtToken(resultAccessToken);
            new JwtSecurityTokenHandler().ReadJwtToken(resultRefreshToken);

            Assert.NotNull(_dbContext.RefreshTokens.FirstOrDefault(x => x.Token == resultRefreshToken));
            CleanUp();
        }

        private void CleanUp()
        {
            _dbContext?.Database.EnsureDeleted();
            _dbContext?.Dispose();
        }
        
        private static IEnumerable<Account> GetTestAccountsList()
        {
            return new List<Account>()
            {
                new Account() {Id = 1, Email = "test@test.ru", Username = "test", IsBanned = false, Salt = new byte[64]},
                new Account() {Id = 2, Email = "test2@test.ru", Username = "test2", IsBanned = false, Salt = new byte[64]}
            };
        }

        private static Mock<IOptions<JwtOptions>> SetupJwtOptions()
        {
            var jwtOptions = new Mock<IOptions<JwtOptions>>();
            jwtOptions.Setup(x => x.Value).Returns(new JwtOptions()
            {
                Key = "testsetetsetestsetsetZTeteststestsdfgfdhgsjgfdjhsdtgjsdtgjsgjsgfjfsjfgsjhnytdkjmuhtkdtcf",
                RefreshKey = "tkeytsdfsdfsdgahreahestsetsetseatgarwethgfdhfghfdghfgxhbijfdghbjofgdhbjfohbjfdgohjfdgohgrewasvgyt53w",
                Issuer = "https://localhost:3000",
                Audience = "https://localhost:3000",
                AccessTokenLifeTimeInMinutes = 50,
                RefreshTokenLifeTimeInDays = 50,
            });

            return jwtOptions;
        }
        
    }
}

