using System.Linq;
using System.Threading;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Vtodo.UseCases.Handlers.Tasks.Queries.GetTaskHeaderBackground;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Tasks.Queries
{
    public class GetTaskHeaderBackgroundRequestHandlerTest
    {
         private AppDbContext _dbContext = null!;

        [Fact]
        public async void Handle_TaskNotFound_TaskNotFoundException()
        {
            SetupDbContext();

            var request = new GetTaskHeaderBackgroundRequest() {Id = 2};

            var getTaskHeaderBackgroundRequestHandler = new GetTaskHeaderBackgroundRequestHandler(_dbContext, SetupProjectFilesServiceMock().Object);

            await Assert.ThrowsAsync<TaskNotFoundException>(() => getTaskHeaderBackgroundRequestHandler.Handle(request, CancellationToken.None));
            CleanUp();
        } 
        
        private Mock<IProjectsFilesService> SetupProjectFilesServiceMock()
        {
            var mock = new Mock<IProjectsFilesService>();
            mock.Setup(x => x.GetProjectFile(It.IsAny<Project>(), It.IsAny<TaskM>(), It.IsAny<string>()));

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
            
            _dbContext.Tasks.Add(new TaskM() { 
                Title = "Test upload background image",
                Description = "Test update task",
                Priority = TaskPriority.None,
                PrioritySort = 0,
                Board = _dbContext.Boards.First(x => x.Id == 1)
            });
            _dbContext.SaveChanges();
        }
        
        private void CleanUp()
        {
            _dbContext?.Database.EnsureDeleted();
            _dbContext?.Dispose();
        }
    }
}