using System.Linq;
using System.Threading;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Vtodo.UseCases.Handlers.Tasks.Commands.MoveTaskToAnotherTask;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Tasks.Commands
{
    public class MoveTaskToAnotherTaskRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulMoveTaskToAnotherTask_ReturnsSystemTask()
        {
            SetupDbContext();

            var request = new MoveTaskToAnotherTaskRequest() { TaskId = 1, NewParentTaskId = 2};
            var moveTaskToAnotherTaskRequestHandler = new MoveTaskToAnotherTaskRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            moveTaskToAnotherTaskRequestHandler.Handle(request, CancellationToken.None);
            
            Assert.Null(_dbContext.Tasks.FirstOrDefault(x => x.Id == request.TaskId && x.ParentTask == null));
            Assert.NotNull(_dbContext.Tasks.FirstOrDefault(x => x.Id == request.TaskId && x.ParentTask != null && x.ParentTask.Id == request.NewParentTaskId));
            
            CleanUp();
        }

        [Fact]
        public async void Handle_TaskNotFound_ThrowsTaskNotFoundException()
        {
            SetupDbContext();
            
            var request = new MoveTaskToAnotherTaskRequest() { TaskId = 10, NewParentTaskId = 2};
            var moveTaskToAnotherTaskRequestHandler = new MoveTaskToAnotherTaskRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            await Assert.ThrowsAsync<TaskNotFoundException>(() => moveTaskToAnotherTaskRequestHandler.Handle(request, CancellationToken.None));
            
            CleanUp();
        }

        [Fact]
        public async void Handle_TaskIdEqualNewParentTaskId_ThrowsTaskIdEqualNewParentTaskIdException()
        {
            SetupDbContext();
            
            var request = new MoveTaskToAnotherTaskRequest() { TaskId = 1, NewParentTaskId = 1};
            var moveTaskToAnotherTaskRequestHandler = new MoveTaskToAnotherTaskRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            await Assert.ThrowsAsync<TaskIdEqualNewParentTaskIdException>(() => moveTaskToAnotherTaskRequestHandler.Handle(request, CancellationToken.None));
            
            CleanUp();
        }

        [Fact]
        public async void Handle_NewParentTaskIdEqualOldId_ThrowsNewParentTaskIdEqualOldIdException()
        {
            SetupDbContext();
            
            var request = new MoveTaskToAnotherTaskRequest() { TaskId = 3, NewParentTaskId = 1};
            var moveTaskToAnotherTaskRequestHandler = new MoveTaskToAnotherTaskRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            await Assert.ThrowsAsync<NewParentTaskIdEqualOldIdException>(() => moveTaskToAnotherTaskRequestHandler.Handle(request, CancellationToken.None));
            
            CleanUp();
        }
        
        [Fact]
        public async void Handle_NewParentTaskNotFound_ThrowsTaskNotFoundException()
        {
            SetupDbContext();
            
            var request = new MoveTaskToAnotherTaskRequest() { TaskId = 1, NewParentTaskId = 100};
            var moveTaskToAnotherTaskRequestHandler = new MoveTaskToAnotherTaskRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            await Assert.ThrowsAsync<TaskNotFoundException>(() => moveTaskToAnotherTaskRequestHandler.Handle(request, CancellationToken.None));
            
            CleanUp();
        }
        
        [Fact]
        public async void Handle_AttemptMoveTaskToAnotherBoard_ThrowsAnotherBoardException()
        {
            SetupDbContext();
            
            var request = new MoveTaskToAnotherTaskRequest() { TaskId = 1, NewParentTaskId = 4};
            var moveTaskToAnotherTaskRequestHandler = new MoveTaskToAnotherTaskRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            await Assert.ThrowsAsync<AnotherBoardException>(() => moveTaskToAnotherTaskRequestHandler.Handle(request, CancellationToken.None));
            
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
                Title = "Test move task",
                Description = "Test move task",
                Priority = TaskPriority.None,
                PrioritySort = 0,
                Board = _dbContext.Boards.First(x => x.Id == 1)
            });
            
            _dbContext.Tasks.Add(new TaskM() { 
                Title = "Test move task 2",
                Description = "Test move task 2",
                Priority = TaskPriority.None,
                PrioritySort = 0,
                Board = _dbContext.Boards.First(x => x.Id == 1)
            });
            _dbContext.SaveChanges();
            
            _dbContext.Tasks.Add(new TaskM() { 
                Title = "Test child task",
                Description = "Test move task",
                Priority = TaskPriority.None,
                PrioritySort = 0,
                Board = _dbContext.Boards.First(x => x.Id == 1),
                ParentTask = _dbContext.Tasks.First(x => x.Id == 1)
            });
            
            _dbContext.Tasks.Add(new TaskM() { 
                Title = "Test another board task",
                Description = "Test move task",
                Priority = TaskPriority.None,
                PrioritySort = 0,
                Board = _dbContext.Boards.First(x => x.Id == 2)
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