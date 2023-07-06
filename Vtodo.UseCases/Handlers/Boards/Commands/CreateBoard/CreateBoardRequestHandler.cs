using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.Boards.Commands.CreateBoard
{
    internal class CreateBoardRequestHandler : IRequestHandler<CreateBoardRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;
        private readonly IMapper _mapper;
        
        public CreateBoardRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
            _mapper = mapper;
        }
        
        public async Task Handle(CreateBoardRequest request, CancellationToken cancellationToken)
        {
            var createDto = request.CreateBoardDto;
            var board = _mapper.Map<Board>(createDto);

            var project = await _dbContext.Projects.FindAsync(createDto.ProjectId, cancellationToken);
            if (project == null) throw new ProjectNotFoundException();
            
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectUpdate);
            
            board.Project = project;
            
            _dbContext.Boards.Add(board);

            await _dbContext.SaveChangesAsync(cancellationToken);

        }
    }
}