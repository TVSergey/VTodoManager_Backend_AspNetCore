using System.Threading;
using System.Threading.Tasks;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.ProjectsRoles.Commands.RevokeRole
{
    internal class RevokeRoleRequestHandler : IRequestHandler<RevokeRoleRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;

        public RevokeRoleRequestHandler(
            IDbContext dbContext,
            IProjectSecurityService projectSecurityService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
        }
        
        public async Task Handle(RevokeRoleRequest request, CancellationToken cancellationToken)
        {
            var revokeRoleDto = request.RevokeRoleDto;

            var project = await _dbContext.Projects.FindAsync(request.ProjectId, cancellationToken);
            if (project == null) throw new ProjectNotFoundException();
            
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectAdmin);
            
            var account = await _dbContext.Accounts.FindAsync(revokeRoleDto.AccountId);
            if (account == null) throw new AccountNotFoundException();

            _projectSecurityService.RevokeRole(project, account, revokeRoleDto.Role);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}