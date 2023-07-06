using System.Threading;
using System.Threading.Tasks;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.Tasks.Commands.MoveTaskToAnotherTask
{
    internal class MoveTaskToAnotherTaskRequestHandler : IRequestHandler<MoveTaskToAnotherTaskRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;

        public MoveTaskToAnotherTaskRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
        }
        
        public async Task Handle(MoveTaskToAnotherTaskRequest request, CancellationToken cancellationToken)
        {
            var task = await _dbContext.Tasks
                .Include(t => t.Board)
                .Include(t => t.Board.Project)
                .Include(t => t.ParentTask)
                .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);
            if (task == null) throw new TaskNotFoundException();
            
            _projectSecurityService.CheckAccess(task.Board.Project, ProjectRoles.ProjectUpdate);
            
            if (task.Id == request.NewParentTaskId) throw new TaskIdEqualNewParentTaskIdException();

            if(task.ParentTask != null)
                if (task.ParentTask.Id == request.NewParentTaskId) throw new NewParentTaskIdEqualOldIdException();

            var newParentTask = await _dbContext.Tasks
                .Include(t => t.Board)
                .FirstOrDefaultAsync(x => x.Id == request.NewParentTaskId, cancellationToken);
            
            if (newParentTask == null) throw new TaskNotFoundException("New parent task not found");
            if (task.Board.Id != newParentTask.Board.Id) throw new AnotherBoardException();
            
            task.ParentTask = newParentTask;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}