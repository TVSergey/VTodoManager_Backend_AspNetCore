using System.Threading;
using System.Threading.Tasks;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.ProjectsRoles.Commands.ChangeOwner
{
    internal class ChangeOwnerRequestHandler : IRequestHandler<ChangeOwnerRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;

        public ChangeOwnerRequestHandler(
            IDbContext dbContext,
            IProjectSecurityService projectSecurityService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
        }
        
        public async Task Handle(ChangeOwnerRequest request, CancellationToken cancellationToken)
        {
            var changeOwner = request.ChangeOwnerDto;

            var project = await _dbContext.Projects.FindAsync(request.ProjectId, cancellationToken);
            if (project == null) throw new ProjectNotFoundException();
            
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectAdmin);
            
            var account = await _dbContext.Accounts.FindAsync(changeOwner.AccountId);
            if (account == null) throw new AccountNotFoundException();

            _projectSecurityService.ChangeOwner(project, account);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}