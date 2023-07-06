using System.IO;
using System.Linq;
using System.Threading;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Vtodo.UseCases.Handlers.Boards.Commands.UploadBoardHeaderBackground;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Boards.Commands
{
    public class UploadBoardHeaderBackgroundRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulUploadProjectFile_ReturnsTask()
        {
            SetupDbContext();

            var request = new UploadBoardHeaderBackgroundRequest() {Id = 1, BackgroundImage = new MemoryStream(), FileName = "test.png"};
            
            var uploadBoardHeaderBackgroundRequestHandler = new UploadBoardHeaderBackgroundRequestHandler(
                _dbContext, 
                SetupProjectSecurityServiceMock().Object, 
                SetupProjectFilesServiceMock().Object);

            uploadBoardHeaderBackgroundRequestHandler.Handle(request, CancellationToken.None);

            Assert.NotNull(_dbContext.ProjectBoardsFiles.FirstOrDefault(x =>
                x.BoardId == request.Id && x.FileName == request.FileName));

            CleanUp();
        }
        
        [Fact]
        public async void Handle_BoardNotFound_ThrowsBoardNotFoundException()
        {
            SetupDbContext();

            var request = new UploadBoardHeaderBackgroundRequest() {Id = 2, BackgroundImage = new MemoryStream(), FileName = "test.png"};
            
            var uploadBoardHeaderBackgroundRequestHandler = new UploadBoardHeaderBackgroundRequestHandler(
                _dbContext, 
                SetupProjectSecurityServiceMock().Object, 
                SetupProjectFilesServiceMock().Object);

            await Assert.ThrowsAsync<BoardNotFoundException>(() => uploadBoardHeaderBackgroundRequestHandler.Handle(request, CancellationToken.None));

            CleanUp();
        }
        
        private static Mock<IProjectSecurityService> SetupProjectSecurityServiceMock()
        {
            var mock = new Mock<IProjectSecurityService>();
            mock.Setup(x => x.CheckAccess(It.IsAny<Project>(), It.IsAny<ProjectRoles>())).Verifiable();
            
            return mock;
        }
        
        private Mock<IProjectsFilesService> SetupProjectFilesServiceMock()
        {
            var mock = new Mock<IProjectsFilesService>();
            mock.Setup(x => x.CheckFile(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string[]>())).Returns(".png");
            mock.Setup(x => x.UploadProjectFile(It.IsAny<Project>(), It.IsAny<Board>(), It.IsAny<Stream>() , It.IsAny<string>()))
                .Callback(
                (Project project, Board board, Stream steam, string extension) =>
                {
                    _dbContext.ProjectBoardsFiles.Add(new ProjectBoardFile() { BoardId = board.Id, ProjectId = project.Id, FileName = "test.png"});
                    _dbContext.SaveChanges();
                }
                )
                .Returns("test.png");
            
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