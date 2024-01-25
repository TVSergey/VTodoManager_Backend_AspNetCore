using System.IO;
using System.Linq;
using System.Threading;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Vtodo.UseCases.Handlers.Boards.Queries.GetBoardHeaderBackground;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Boards.Queries
{
    public class GetBoardHeaderBackgroundRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public async void Handle_BoardNotFound_BoardNotFoundException()
        {
            SetupDbContext();

            var request = new GetBoardHeaderBackgroundRequest() {Id = 2};

            var getBoardHeaderBackgroundRequestHandler = new GetBoardHeaderBackgroundRequestHandler(_dbContext, SetupProjectFilesServiceMock().Object);

            await Assert.ThrowsAsync<BoardNotFoundException>(() => getBoardHeaderBackgroundRequestHandler.Handle(request, CancellationToken.None));
            CleanUp();
        } 

        private Mock<IProjectsFilesService> SetupProjectFilesServiceMock()
        {
            var mock = new Mock<IProjectsFilesService>();
            mock.Setup(x => x.GetProjectFile(It.IsAny<Project>(), It.IsAny<Board>(), It.IsAny<string>()));

            return mock;
        }
        
        private void SetupDbContext()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();
            
            _dbContext.Projects.Add(new Project() {Title = "Test Project"});
            _dbContext.Projects.Add(new Project() {Title = "Test Project2"});
            _dbContext.SaveChanges();
            
            _dbContext.Boards.Add(new Board() {Title = "Test Board", PrioritySort = 0, Project = _dbContext.Projects.First()});
            _dbContext.SaveChanges();
        }
        
        private void CleanUp()
        {
            _dbContext?.Database.EnsureDeleted();
            _dbContext?.Dispose();
        }
    }
}