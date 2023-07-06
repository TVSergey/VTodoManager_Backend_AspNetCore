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

namespace Vtodo.UseCases.Handlers.Accounts.Commands.Logout
{
    internal class LogoutRequestHandler : IRequestHandler<LogoutRequest>
    {
        private readonly IJwtService _jwtService;
        private readonly ICurrentAccountService _currentAccountService;
        
        public LogoutRequestHandler(IJwtService jwtService, ICurrentAccountService currentAccountService)
        {
            _jwtService = jwtService;
            _currentAccountService = currentAccountService;
        }
        
        public async Task Handle(LogoutRequest request, CancellationToken cancellationToken)
        {
            _jwtService.InvalidateRefreshToken(_currentAccountService.Account, request.LogoutDto.RefreshToken);
        }
    }
}