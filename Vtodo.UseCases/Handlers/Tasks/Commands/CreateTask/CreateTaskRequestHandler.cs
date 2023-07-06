using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.Tasks.Commands.CreateTask
{
    internal class CreateTaskRequestHandler : IRequestHandler<CreateTaskRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;
        private readonly IMapper _mapper;
        
        public CreateTaskRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
            _mapper = mapper;
        }
        
        public async Task Handle(CreateTaskRequest request, CancellationToken cancellationToken)
        {
            var createDto = request.CreateTaskDto;
            var task = _mapper.Map<TaskM>(createDto);

            var board = await _dbContext.Boards
                .Include(x => x.Project)
                .FirstOrDefaultAsync(x => x.Id == createDto.BoardId, cancellationToken);

            if (board == null) throw new BoardNotFoundException();
            _projectSecurityService.CheckAccess(board.Project, ProjectRoles.ProjectUpdate);
            
            task.Board = board;

            if (createDto.ParentTaskId != null)
            {
                var parentTask = await _dbContext.Tasks.FindAsync(createDto.ParentTaskId, cancellationToken);
                if (parentTask == null) throw new TaskNotFoundException("Parent task not found");

                task.ParentTask = parentTask;
            }
            _dbContext.Tasks.Add(task);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}