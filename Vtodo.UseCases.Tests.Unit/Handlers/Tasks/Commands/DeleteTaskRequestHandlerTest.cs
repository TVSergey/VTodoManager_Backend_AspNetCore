using System.Linq;
using System.Threading;
using AutoMapper;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Vtodo.UseCases.Handlers.Tasks.Commands.DeleteTask;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Tasks.Commands
{
    public class DeleteTaskRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        private void Handle_SuccessfulDeleteTask_ReturnsSystemTask()
        {
            SetupDbContext();

            var request = new DeleteTaskRequest() {Id = 1};

            var deleteTaskRequestHandler = new DeleteTaskRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);
            deleteTaskRequestHandler.Handle(request, CancellationToken.None);
            
            Assert.Null(_dbContext.Tasks.FirstOrDefault(x => x.Id == 1));
            
            CleanUp();
        }
        
        [Fact]
        private async void Handle_TaskNotFound_ThrowsTaskNotFoundException()
        {
            SetupDbContext();

            var request = new DeleteTaskRequest() {Id = 50};

            var deleteTaskRequestHandler = new DeleteTaskRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);
           
            await Assert.ThrowsAsync<TaskNotFoundException>(() => deleteTaskRequestHandler.Handle(request, CancellationToken.None));

            CleanUp();
        }
        
        private static Mock<IProjectSecurityService> SetupProjectSecurityServiceMock()
        {
            var mock = new Mock<IProjectSecurityService>();
            mock.Setup(x => x.CheckAccess(It.IsAny<Project>(), It.IsAny<ProjectRoles>())).Verifiable();
            
            return mock;
        }

        private void SetupDbContext()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();

            _dbContext.Accounts.Add(new Account() { Email = "test@test.ru", Username = "test", HashedPassword = "test" , Salt = new byte[64]});
            
            _dbContext.Projects.Add(new Project() {Title = "Test Project"});
            _dbContext.SaveChanges();
            
            _dbContext.Boards.Add(new Board() {Title = "Test Project", Project = _dbContext.Projects.First(), PrioritySort = 0});
            _dbContext.SaveChanges();

            _dbContext.Tasks.Add(new TaskM() { 
                Title = "Test delete task",
                Description = "Test delete task",
                Priority = TaskPriority.None,
                PrioritySort = 0,
                Board = _dbContext.Boards.First(x => x.Id == 1)
            });
            _dbContext.SaveChanges();
            
            _dbContext.Tasks.Add(new TaskM() { 
                Title = "Test child task",
                Description = "Test delete task",
                Priority = TaskPriority.None,
                PrioritySort = 0,
                Board = _dbContext.Boards.First(x => x.Id == 1),
                ParentTask = _dbContext.Tasks.First(x => x.Id == 1)
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