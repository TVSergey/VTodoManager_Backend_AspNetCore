using System.Threading;
using System.Threading.Tasks;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.Boards.Commands.DeleteBoard
{
    internal class DeleteBoardRequestHandler : IRequestHandler<DeleteBoardRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;

        public DeleteBoardRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
        }
        
        public async Task Handle(DeleteBoardRequest request, CancellationToken cancellationToken)
        {
            var board = await _dbContext.Boards
                .Include(t => t.Project)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (board == null) throw new BoardNotFoundException();

            _projectSecurityService.CheckAccess(board.Project, ProjectRoles.ProjectUpdate);
            
            _dbContext.Boards.Remove(board);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}