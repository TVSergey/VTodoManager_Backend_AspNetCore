using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.Tasks.Commands.DeleteTaskHeaderBackground
{
    internal class DeleteTaskHeaderBackgroundRequestHandler : IRequestHandler<DeleteTaskHeaderBackgroundRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;
        private readonly IProjectsFilesService _projectFilesService;

        public DeleteTaskHeaderBackgroundRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService,
            IProjectsFilesService projectFilesService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
            _projectFilesService = projectFilesService;
        }
        
        public async Task Handle(DeleteTaskHeaderBackgroundRequest request, CancellationToken cancellationToken)
        {
            var task = await _dbContext.Tasks
                .Include(t => t.Board)
                .Include(t => t.Board.Project)
                .Include(t => t.ParentTask)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (task == null) throw new TaskNotFoundException();

            _projectSecurityService.CheckAccess(task.Board.Project, ProjectRoles.ProjectUpdate);

            if (string.IsNullOrWhiteSpace(task.ImageHeaderPath)) return;
            _projectFilesService.DeleteProjectFile(task.Board.Project, task, task.ImageHeaderPath);

            task.ImageHeaderPath = null;
            
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}