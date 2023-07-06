using System.Threading;
using System.Threading.Tasks;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.Projects.Commands.UpdateProject
{
    internal class UpdateProjectRequestHandler : IRequestHandler<UpdateProjectRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;
        
        public UpdateProjectRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
        }
        
        public async Task Handle(UpdateProjectRequest request, CancellationToken cancellationToken)
        {
            var project = await _dbContext.Projects.FindAsync(request.Id, cancellationToken);

            if (project == null)  throw new ProjectNotFoundException();

            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectUpdate);
            
            var updateDto = request.UpdateProjectDto;
            project.Title = updateDto.Title;
            
            await _dbContext.SaveChangesAsync(cancellationToken);

        }
    }
}