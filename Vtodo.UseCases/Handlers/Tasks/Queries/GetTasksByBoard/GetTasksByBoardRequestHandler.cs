using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Vtodo.UseCases.Handlers.Tasks.Queries.GetTasksByBoard
{
    internal class GetTasksByBoardRequestHandler : IRequestHandler<GetTasksByBoardRequest, List<TaskDto>>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;
        private readonly IMapper _mapper;
        
        public GetTasksByBoardRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
            _mapper = mapper;
        }
        
        public async Task<List<TaskDto>> Handle(GetTasksByBoardRequest request, CancellationToken cancellationToken)
        {
            var board = await _dbContext.Boards
                .Include(x => x.Project)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.BoardId, cancellationToken: cancellationToken);
            if (board == null) throw new BoardNotFoundException();

            var tasks = await _dbContext.Tasks
                .Include(x => x.Board)
                .Include(x => x.ParentTask)
                .AsNoTracking()
                .Where(x => x.Board.Id == board.Id)
                .ToListAsync(cancellationToken: cancellationToken);

            _projectSecurityService.CheckAccess(board.Project, ProjectRoles.ProjectMember);
            
            var result = _mapper.Map<List<TaskDto>>(tasks);

            for (var i = 0; i < tasks.Count; i++)
            {
                var task = tasks.ElementAt(i);
                if (task.EndDate != null) result[i].EndDate = new DateTimeOffset((DateTime) task.EndDate).ToUnixTimeMilliseconds();

                result[i].BoardId = task.Board.Id;
                result[i].ParentId = task.ParentTask?.Id;
            }

            return result;
        }
    }
}