using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.Infrastructure.Implementation.Services
{
    internal class CurrentAccountService : ICurrentAccountService
    {
        public CurrentAccountService(IHttpContextAccessor httpContextAccessor, IDbContext dbContext)
        {
            if (!(httpContextAccessor.HttpContext.User?.Identity?.IsAuthenticated ?? false)) throw new UnauthorizedException();
            
            int.TryParse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);

            Account = dbContext.Accounts.FirstOrDefault(x => x.Id == userId) ?? throw new UnauthorizedException();
        }
        
        public Account Account { get; set; }
    }
}