using System.Linq;
using System.Threading;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Vtodo.UseCases.Handlers.Boards.Commands.MoveBoardToAnotherProject;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Boards.Commands
{
    public class MoveBoardToAnotherProjectRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulMoveBoardToAnotherProject_ReturnsTask()
        {
            SetupDbContext();

            var request = new MoveBoardToAnotherProjectRequest() { BoardId = 1, ProjectId = 2};
            
            var moveBoardToAnotherProjectRequestHandler = new MoveBoardToAnotherProjectRequestHandler(_dbContext, SetupProjectSecurityService().Object);

            moveBoardToAnotherProjectRequestHandler.Handle(request, CancellationToken.None);
            
            Assert.Null(_dbContext.Boards.FirstOrDefault(x => x.Project == _dbContext.Projects.First(d => d.Id == 1)));
            Assert.NotNull(_dbContext.Boards.FirstOrDefault(x => x.Project == _dbContext.Projects.First(d => d.Id == 2)));
            
            CleanUp();
        }

        [Fact]
        public async void Handle_BoardNotFound_ThrowsBoardNotFoundException()
        {
            SetupDbContext();

            var request = new MoveBoardToAnotherProjectRequest() { BoardId = 2, ProjectId = 2};
            
            var moveBoardToAnotherProjectRequestHandler = new MoveBoardToAnotherProjectRequestHandler(_dbContext, SetupProjectSecurityService().Object);

            await Assert.ThrowsAsync<BoardNotFoundException>(() => moveBoardToAnotherProjectRequestHandler.Handle(request, CancellationToken.None));

            CleanUp();
        }
        
        [Fact]
        public async void Handle_NewProjectIdEqualOldIdException_ThrowsNewProjectIdEqualOldIdException()
        {
            SetupDbContext();

            var request = new MoveBoardToAnotherProjectRequest() { BoardId = 1, ProjectId = 1};
            
            var moveBoardToAnotherProjectRequestHandler = new MoveBoardToAnotherProjectRequestHandler(_dbContext, SetupProjectSecurityService().Object);

            await Assert.ThrowsAsync<NewProjectIdEqualOldIdException>(() => moveBoardToAnotherProjectRequestHandler.Handle(request, CancellationToken.None));

            CleanUp();
        }
        
        private static Mock<IProjectSecurityService> SetupProjectSecurityService()
        {
            var mock = new Mock<IProjectSecurityService>();
            mock.Setup(x => x.CheckAccess(It.IsAny<Project>(), It.IsAny<ProjectRoles>())).Verifiable();
            
            return new Mock<IProjectSecurityService>();
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