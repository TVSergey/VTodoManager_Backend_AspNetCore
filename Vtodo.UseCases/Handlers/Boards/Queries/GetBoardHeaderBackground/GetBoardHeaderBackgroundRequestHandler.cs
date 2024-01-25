using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.Boards.Queries.GetBoardHeaderBackground
{
    internal class GetBoardHeaderBackgroundRequestHandler : IRequestHandler<GetBoardHeaderBackgroundRequest, FileStream?>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectsFilesService _projectFilesService;
        
        public GetBoardHeaderBackgroundRequestHandler(
            IDbContext dbContext,
            IProjectsFilesService projectFilesService)
        {
            _dbContext = dbContext;
            _projectFilesService = projectFilesService;
        }
        
        public async Task<FileStream?> Handle(GetBoardHeaderBackgroundRequest request, CancellationToken cancellationToken)
        {
            var board = await _dbContext.Boards
                .Include(x => x.Project)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);

            if (board == null) throw new BoardNotFoundException();

            if (board.ImageHeaderPath == null) return null;

            var result = _projectFilesService.GetProjectFile(board.Project, board, board.ImageHeaderPath);

            return result;
        }
    }
}