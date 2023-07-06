using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.Boards.Commands.DeleteBoardHeaderBackground
{
    internal class DeleteBoardHeaderBackgroundRequestHandler : IRequestHandler<DeleteBoardHeaderBackgroundRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;
        private readonly IProjectsFilesService _projectFilesService;

        public DeleteBoardHeaderBackgroundRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService,
            IProjectsFilesService projectFilesService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
            _projectFilesService = projectFilesService;
        }
        
        public async Task Handle(DeleteBoardHeaderBackgroundRequest request, CancellationToken cancellationToken)
        {
            var board = await _dbContext.Boards
                .Include(t => t.Project)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (board == null) throw new BoardNotFoundException();

            _projectSecurityService.CheckAccess(board.Project, ProjectRoles.ProjectUpdate);

            if (string.IsNullOrWhiteSpace(board.ImageHeaderPath)) throw new AttemptGetNullFileException();
            _projectFilesService.DeleteProjectFile(board.Project, board, board.ImageHeaderPath);

            board.ImageHeaderPath = null;
            
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}