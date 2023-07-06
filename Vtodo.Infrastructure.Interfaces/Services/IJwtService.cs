using Vtodo.Entities.Models;

namespace Vtodo.Infrastructure.Interfaces.Services
{
    internal interface IJwtService
    {
        string GenerateToken(Account account);
        string GenerateRefreshToken(Account account, string clientIp, string device);
        void InvalidateRefreshToken(Account currentAccount, string token);
        void InvalidateAllRefreshTokens(Account account);
        string RefreshTokens(string refreshToken, out string newRefreshToken);
        string GenerateNewTokensAfterLogin(Account account, out string newRefreshToken);

    }
}