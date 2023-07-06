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
using Vtodo.UseCases.Handlers.Tasks.Commands.MoveTaskToAnotherBoard;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Tasks.Commands
{
    public class MoveTaskToAnotherBoardRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulMoveTaskToAnotherBoard_ReturnsSystemTask()
        {
            SetupDbContext();

            var request = new MoveTaskToAnotherBoardRequest() { TaskId = 1, NewBoardId = 2};
            var moveTaskToAnotherBoardRequestHandler = new MoveTaskToAnotherBoardRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            moveTaskToAnotherBoardRequestHandler.Handle(request, CancellationToken.None);
            
            Assert.Null(_dbContext.Tasks.FirstOrDefault(x => x.Id == request.TaskId && x.Board.Id == 1));
            Assert.NotNull(_dbContext.Tasks.FirstOrDefault(x => x.Id == request.TaskId && x.Board.Id == 2));
            
            Assert.Null(_dbContext.Tasks.FirstOrDefault(x => x.Id == 2 && x.Board.Id == 1));
            Assert.NotNull(_dbContext.Tasks.FirstOrDefault(x => x.Id == 2 && x.Board.Id == 2));
            
            CleanUp();
        }
        
        [Fact]
        public async void Handle_BoardNotFound_ThrowsBoardNotFoundException()
        {
            SetupDbContext();

            var request = new MoveTaskToAnotherBoardRequest() { TaskId = 1, NewBoardId = 20};
            var moveTaskToAnotherBoardRequestHandler = new MoveTaskToAnotherBoardRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            await Assert.ThrowsAsync<BoardNotFoundException>(() => moveTaskToAnotherBoardRequestHandler.Handle(request, CancellationToken.None));

            CleanUp();
        }

        [Fact]
        public async void Handle_TaskNotFound_ThrowsTaskNotFoundException()
        {
            SetupDbContext();

            var request = new MoveTaskToAnotherBoardRequest() { TaskId = 100, NewBoardId = 2};
            var moveTaskToAnotherBoardRequestHandler = new MoveTaskToAnotherBoardRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            await Assert.ThrowsAsync<TaskNotFoundException>(() => moveTaskToAnotherBoardRequestHandler.Handle(request, CancellationToken.None));

            CleanUp();
        }
        
        [Fact]
        public async void Handle_NewBoardIdEqualOldId_ThrowsNewBoardIdEqualOldIdException()
        {
            SetupDbContext();

            var request = new MoveTaskToAnotherBoardRequest() { TaskId = 1, NewBoardId = 1};
            var moveTaskToAnotherBoardRequestHandler = new MoveTaskToAnotherBoardRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            await Assert.ThrowsAsync<NewBoardIdEqualOldIdException>(() => moveTaskToAnotherBoardRequestHandler.Handle(request, CancellationToken.None));

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
            _dbContext.SaveChanges();
            
            _dbContext.Tasks.Add(new TaskM() { 
                Title = "Test child task",
                Description = "Test move task",
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