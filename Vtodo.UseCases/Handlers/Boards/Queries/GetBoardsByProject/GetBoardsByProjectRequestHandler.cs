using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.UseCases.Handlers.Boards.Dto;

namespace Vtodo.UseCases.Handlers.Boards.Queries.GetBoardsByProject
{
    internal class GetBoardsByProjectRequestHandler : IRequestHandler<GetBoardsByProjectRequest, List<BoardDto>>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;
        private readonly IMapper _mapper;
        
        public GetBoardsByProjectRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
            _mapper = mapper;
        }
        
        public async Task<List<BoardDto>> Handle(GetBoardsByProjectRequest request, CancellationToken cancellationToken)
        {
            var project = await _dbContext.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken: cancellationToken);
            if (project == null) throw new ProjectNotFoundException();
            
            var boards = await _dbContext.Boards
                .Include(x => x.Project)
                .AsNoTracking()
                .Where(x => x.Project.Id == project.Id)
                .ToListAsync(cancellationToken: cancellationToken);
            
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectMember);
            
            var result = _mapper.Map<List<BoardDto>>(boards);

            for (var i = 0; i < boards.Count; i++)
            {
                var board = boards[i];
                result[i].ProjectId = board.Project.Id;
            }
            
            return result;
        }
    }
}