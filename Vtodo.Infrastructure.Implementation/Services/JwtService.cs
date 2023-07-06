using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Implementation.Options;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.Infrastructure.Implementation.Services
{
    internal class JwtService : IJwtService
    {
        private readonly IDbContext _dbContext;
        private readonly JwtOptions _jwtOptions;
        private readonly IClientInfoService _clientInfoService;
        
        public JwtService(IDbContext dbContext, IOptions<JwtOptions> jwtOptions, IClientInfoService clientInfoService)
        {
            _dbContext = dbContext;
            _jwtOptions = jwtOptions.Value;
            _clientInfoService = clientInfoService;
        }

        public string GenerateToken(Account account)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            
            var tokenOptions = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenLifeTimeInMinutes),
                    signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256),
                    claims: new List<Claim>()
                    {
                       new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                       new Claim("email", account.Email),
                       new Claim("username", account.Username),
                    }
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public string GenerateRefreshToken(Account account, string clientIp, string device)
        { 
           var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.RefreshKey));
           var expireAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenLifeTimeInDays);
           var tokenOptions = new JwtSecurityToken(
                   issuer: _jwtOptions.Issuer,
                   audience: _jwtOptions.Audience,
                   expires: expireAt,
                   signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256),
                   claims: new List<Claim>()
                   {
                      new Claim("id", account.Id.ToString()),
                      new Claim("email", account.Email),
                      new Claim("username", account.Username),
                   }
           );
           var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
           _dbContext.RefreshTokens.Add(new RefreshToken()
           {
               Token = token,
               AccountId = account.Id,
               ExpireAt = expireAt,
               Ip = clientIp,
               Device = device
           });

           _dbContext.SaveChanges();
           
           return token;
        }

        public void InvalidateRefreshToken(Account currentAccount, string token)
        {
            var tAccount = GetAccountByToken(token);
            if (tAccount  == null) throw new AccessDeniedException();

            CheckAccessToToken(currentAccount, tAccount);
            
            var refreshToken = _dbContext.RefreshTokens
                .FirstOrDefault(x => x.Token == token);
            if (refreshToken == null) throw new InvalidTokenException();

            _dbContext.RefreshTokens.Remove(refreshToken);
            _dbContext.SaveChanges();
            
            if (refreshToken.ExpireAt <= DateTime.Now) throw new ExpiredTokenException();
        }

        public void InvalidateAllRefreshTokens(Account currentAccount)
        {
            var refreshTokens = _dbContext.RefreshTokens.Where(x => x.AccountId == currentAccount.Id);

            _dbContext.RefreshTokens.RemoveRange(refreshTokens);
            _dbContext.SaveChanges();
        }
        
        public string RefreshTokens(string refreshToken, out string newRefreshToken)
        {
            var account = GetAccountByToken(refreshToken);
            if (account == null) throw new AccessDeniedException();

            InvalidateRefreshToken(account, refreshToken);

            return GenerateTokens(account, out newRefreshToken);
        }

        public string GenerateNewTokensAfterLogin(Account account, out string newRefreshToken)
        {
            return GenerateTokens(account, out newRefreshToken);
        }
        
        private void CheckAccessToToken(Account currentAccount, Account tokenAccount)
        {
            if (currentAccount.Id != tokenAccount.Id) throw new AccessDeniedException();
        }
        
        private string GenerateTokens(Account account, out string refreshToken)
        {
            var newAccessToken = GenerateToken(account);
            refreshToken = GenerateRefreshToken(account, _clientInfoService.Ip, _clientInfoService.DeviceInfo);
            
            return newAccessToken;
        }

        private Account? GetAccountByToken(string refreshToken)
        {
            var accountId = _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken)?.AccountId;
            
            if (accountId == null) return null;
            return _dbContext.Accounts.FirstOrDefault(x => x.Id == accountId);
        }
    }
}