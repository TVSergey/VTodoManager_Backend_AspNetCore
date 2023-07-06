using System.Threading;
using System.Threading.Tasks;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.ProjectsRoles.Commands.AddMember
{
    internal class AddMemberRequestHandler : IRequestHandler<AddMemberRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;

        public AddMemberRequestHandler(
            IDbContext dbContext,
            IProjectSecurityService projectSecurityService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
        }
        
        public async Task Handle(AddMemberRequest request, CancellationToken cancellationToken)
        {
            var addMemberDto = request.AddMemberDto;

            var project = await _dbContext.Projects.FindAsync(request.ProjectId, cancellationToken);
            if (project == null) throw new ProjectNotFoundException();
            
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectAdmin);
            
            var account = await _dbContext.Accounts.FindAsync(addMemberDto.AccountId);
            if (account == null) throw new AccountNotFoundException();

            _projectSecurityService.AddMember(project, account);
        }
    }
}