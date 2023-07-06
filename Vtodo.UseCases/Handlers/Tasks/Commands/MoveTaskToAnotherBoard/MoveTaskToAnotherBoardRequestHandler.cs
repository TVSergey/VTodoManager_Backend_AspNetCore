using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.Tasks.Commands.MoveTaskToAnotherBoard
{
    internal class MoveTaskToAnotherBoardRequestHandler : IRequestHandler<MoveTaskToAnotherBoardRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;

        public MoveTaskToAnotherBoardRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
        }
        
        public async Task Handle(MoveTaskToAnotherBoardRequest request, CancellationToken cancellationToken)
        {
            var newBoard = await _dbContext.Boards
                .Include(x => x.Project)
                .FirstOrDefaultAsync(x => x.Id == request.NewBoardId, cancellationToken: cancellationToken);
            if (newBoard == null) throw new BoardNotFoundException();

            var rootTask = _dbContext.Tasks
                .Include(t => t.Board)
                .Include(t => t.Board.Project)
                .Include(t=> t.ParentTask)
                .Include(t => t.ChildrenTasks)
                .FirstOrDefault(x => x.Id == request.TaskId);
            if (rootTask == null) throw new TaskNotFoundException();
            
            _projectSecurityService.CheckAccess(rootTask.Board.Project, ProjectRoles.ProjectUpdate);
            _projectSecurityService.CheckAccess(newBoard.Project, ProjectRoles.ProjectUpdate);
            
            if (rootTask.Board.Id == newBoard.Id) throw new NewBoardIdEqualOldIdException();

            rootTask.ParentTask = null;

            var tasksList = new List<TaskM> {rootTask};

            var childrenIterationList = await _dbContext.Tasks
                .Where(x => x.ParentTask == rootTask)
                .ToListAsync(cancellationToken: cancellationToken);
            
            while (childrenIterationList.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                tasksList.AddRange(childrenIterationList);
                
                var childrenIterationIds = childrenIterationList.Select(x => x.Id).ToArray();
                childrenIterationList = await _dbContext.Tasks
                    .Where(x => x.ParentTask != null && childrenIterationIds.Contains(x.ParentTask.Id))
                    .ToListAsync(cancellationToken: cancellationToken);
            }

            foreach (var task in tasksList)
            {
                task.Board = newBoard;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        
    }
}