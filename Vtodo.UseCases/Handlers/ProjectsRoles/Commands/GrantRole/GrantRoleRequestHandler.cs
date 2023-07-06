using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.ProjectsRoles.Commands.GrantRole
{
    internal class GrantRoleRequestHandler : IRequestHandler<GrantRoleRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;

        public GrantRoleRequestHandler(
            IDbContext dbContext,
            IProjectSecurityService projectSecurityService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
        }
        
        public async Task Handle(GrantRoleRequest request, CancellationToken cancellationToken)
        {
            var grantRoleDto = request.GrantRoleDto;
            if (grantRoleDto.Role == ProjectRoles.ProjectMember) throw new AttemptAddMemberFromGrantRoleException();
            
            var project = await _dbContext.Projects.FindAsync(request.ProjectId, cancellationToken);
            if (project == null) throw new ProjectNotFoundException();
            
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectAdmin);
            
            var account = await _dbContext.Accounts.FindAsync(grantRoleDto.AccountId);
            if (account == null) throw new AccountNotFoundException();

            _projectSecurityService.GrantRole(project, account, grantRoleDto.Role);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}