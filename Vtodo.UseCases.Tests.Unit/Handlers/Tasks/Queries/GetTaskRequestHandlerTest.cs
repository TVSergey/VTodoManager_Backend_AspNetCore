using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Vtodo.UseCases.Handlers.Tasks.Dto;
using Vtodo.UseCases.Handlers.Tasks.Queries.GetTask;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Tasks.Queries
{
    public class GetTaskRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulGetTask_ReturnSystemTaskTaskDto()
        {
            SetupDbContext();
            
            var request = new GetTaskRequest() { Id = 1 };
            
            var testTask = _dbContext.Tasks.Include(x => x.Board).First(x => x.Id == request.Id);
            var mapperMock = SetupMapperMock();

            mapperMock.Setup(x => x.Map<TaskDto>(It.IsAny<TaskM>())).Returns(new TaskDto()
            {
                Title = testTask.Title,
                Description = testTask.Description,
                Priority = testTask.PrioritySort,
                PrioritySort = testTask.PrioritySort,
                BoardId = testTask.Board.Id,
            });
            
            var getTaskRequestHandler = new GetTaskRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object, mapperMock.Object);

            var result = getTaskRequestHandler.Handle(request, CancellationToken.None).Result;
            
            Assert.NotNull(result);
            
            CleanUp();
        }
        
        [Fact]
        public async void Handle_TaskNotFound_ThrowsTaskNotFoundException()
        {
            SetupDbContext();
            
            var request = new GetTaskRequest() { Id = 100 };
            var mapperMock = SetupMapperMock();
            
            var getTaskRequestHandler = new GetTaskRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object, mapperMock.Object);

            await Assert.ThrowsAsync<TaskNotFoundException>(() => getTaskRequestHandler.Handle(request, CancellationToken.None));
            
            CleanUp();
        }
        
        private static Mock<IProjectSecurityService> SetupProjectSecurityServiceMock()
        {
            var mock = new Mock<IProjectSecurityService>();
            mock.Setup(x => x.CheckAccess(It.IsAny<Project>(), It.IsAny<ProjectRoles>())).Verifiable();
            
            return mock;
        }

        private static Mock<IMapper> SetupMapperMock()
        {
            return new Mock<IMapper>();
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