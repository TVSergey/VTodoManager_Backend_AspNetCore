using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.UseCases.Handlers.Tasks.Dto;

namespace Vtodo.UseCases.Handlers.Tasks.Queries.GetTask
{
    internal class GetTaskRequestHandler : IRequestHandler<GetTaskRequest, TaskDto>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;
        private readonly IMapper _mapper;
        
        public GetTaskRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
            _mapper = mapper;
        }
        
        public async Task<TaskDto> Handle(GetTaskRequest request, CancellationToken cancellationToken)
        {
            var task = await _dbContext.Tasks
                .Include(x => x.ParentTask)
                .Include(x => x.ChildrenTasks)
                .Include(x => x.Board)
                .Include(x => x.Board.Project)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);

            if (task == null) throw new TaskNotFoundException();

            _projectSecurityService.CheckAccess(task.Board.Project, ProjectRoles.ProjectMember);
            
            var result = _mapper.Map<TaskDto>(task);
            
            if (task.EndDate != null) result.EndDate = new DateTimeOffset((DateTime) task.EndDate).ToUnixTimeMilliseconds();

            result.BoardId = task.Board.Id;
            result.ParentId = task.ParentTask?.Id;
            
            return result;
        }
    }
}