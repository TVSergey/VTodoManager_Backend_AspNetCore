using System.Threading;
using System.Threading.Tasks;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.Boards.Commands.MoveBoardToAnotherProject
{
    internal class MoveBoardToAnotherProjectRequestHandler : IRequestHandler<MoveBoardToAnotherProjectRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;

        public MoveBoardToAnotherProjectRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
        }
        
        public async Task Handle(MoveBoardToAnotherProjectRequest request, CancellationToken cancellationToken)
        {
            var board = await _dbContext.Boards.Include(b => b.Project).FirstOrDefaultAsync(x => x.Id == request.BoardId, cancellationToken);
            if (board == null) throw new BoardNotFoundException();
            
            _projectSecurityService.CheckAccess(board.Project, ProjectRoles.ProjectUpdate);
            
            var newProject = await _dbContext.Projects.FindAsync(request.ProjectId, cancellationToken);

            if (newProject == null) throw new ProjectNotFoundException();
            if (newProject.Id == board.Project.Id) throw new NewProjectIdEqualOldIdException();
            
            _projectSecurityService.CheckAccess(newProject, ProjectRoles.ProjectUpdate);
            
            board.Project = newProject;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}