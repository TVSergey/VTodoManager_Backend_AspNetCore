using System.Threading;
using System.Threading.Tasks;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.Projects.Commands.DeleteProject
{
    internal class DeleteProjectRequestHandler : IRequestHandler<DeleteProjectRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;

        public DeleteProjectRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
        }
        
        public async Task Handle(DeleteProjectRequest request, CancellationToken cancellationToken)
        {
            var project = await _dbContext.Projects.FindAsync(request.Id,cancellationToken);

            if (project == null) throw new ProjectNotFoundException();

            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectOwner);
            
            _dbContext.Projects.Remove(project);

            await _dbContext.SaveChangesAsync(cancellationToken);

        }
    }
}