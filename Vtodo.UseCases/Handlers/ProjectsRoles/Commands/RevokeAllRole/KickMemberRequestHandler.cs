using System.Threading;
using System.Threading.Tasks;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.ProjectsRoles.Commands.RevokeAllRole
{
    internal class KickMemberRequestHandler : IRequestHandler<KickMemberRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;

        public KickMemberRequestHandler(
            IDbContext dbContext,
            IProjectSecurityService projectSecurityService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
        }
        
        public async Task Handle(KickMemberRequest request, CancellationToken cancellationToken)
        {
            var kickMemberDto = request.KickMemberDto;

            var project = await _dbContext.Projects.FindAsync(request.ProjectId, cancellationToken);
            if (project == null) throw new ProjectNotFoundException();
            
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectAdmin);
            
            var account = await _dbContext.Accounts.FindAsync(kickMemberDto.AccountId);
            if (account == null) throw new AccountNotFoundException();

            _projectSecurityService.RevokeAllRoles(project, account);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}