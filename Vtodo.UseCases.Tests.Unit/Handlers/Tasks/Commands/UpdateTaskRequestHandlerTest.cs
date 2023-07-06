using System.Linq;
using System.Threading;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Vtodo.UseCases.Handlers.Tasks.Commands.UpdateTask;
using Vtodo.UseCases.Handlers.Tasks.Dto;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Tasks.Commands
{
    public class UpdateTaskRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulUpdateTask_ReturnsSystemTask()
        {
            SetupDbContext();

            var updateTaskDto = new UpdateTaskDto()
            {
                Title = "Updated task",
                Description = "New description",
                EndDateTimeStamp = 1688595313,
                IsCompleted = true,
                Priority = TaskPriority.Priority1,
                PrioritySort = 1
            };
            
            var request = new UpdateTaskRequest() { Id = 1, UpdateTaskDto = updateTaskDto };
            
            var updateTaskRequestHandler = new UpdateTaskRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            updateTaskRequestHandler.Handle(request, CancellationToken.None);

            Assert.Null(_dbContext.Tasks.FirstOrDefault(x => x.Id == 1 &&
                 x.Title == "Test update task" && 
                 x.Description == "Test update task" &&
                 x.Priority == TaskPriority.None &&
                 x.PrioritySort == 0));
            
            Assert.NotNull(_dbContext.Tasks.FirstOrDefault(x => x.Id == 1 &&
                 x.Title == updateTaskDto.Title && 
                 x.Description == updateTaskDto.Description &&
                 x.Priority == updateTaskDto.Priority &&
                 x.PrioritySort == updateTaskDto.PrioritySort &&
                 x.IsCompleted == updateTaskDto.IsCompleted &&
                 x.EndDate != null));
             
            CleanUp();
        }
        
        [Fact]
        public async void Handle_TaskNotFound_ThrowsTaskNotFoundException()
        {
            SetupDbContext();

            var updateTaskDto = new UpdateTaskDto()
            {
                Title = "Updated task",
                Description = "New description",
                EndDateTimeStamp = 1688595313,
                IsCompleted = true,
                Priority = TaskPriority.Priority1,
                PrioritySort = 1
            };
            
            var request = new UpdateTaskRequest() { Id = 100, UpdateTaskDto = updateTaskDto };
            
            var updateTaskRequestHandler = new UpdateTaskRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            await Assert.ThrowsAsync<TaskNotFoundException>(() => updateTaskRequestHandler.Handle(request, CancellationToken.None));
            
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
            
            _dbContext.Boards.Add(new Board() {Title = "Test Board 1", Project = _dbContext.Projects.First(), PrioritySort = 0});
            _dbContext.Boards.Add(new Board() {Title = "Test Board 2", Project = _dbContext.Projects.First(), PrioritySort = 0});
            _dbContext.SaveChanges();
            
            _dbContext.Tasks.Add(new TaskM() { 
                Title = "Test update task",
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