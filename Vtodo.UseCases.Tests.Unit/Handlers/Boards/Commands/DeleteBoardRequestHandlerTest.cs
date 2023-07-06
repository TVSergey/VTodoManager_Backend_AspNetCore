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
using Vtodo.UseCases.Handlers.Boards.Commands.DeleteBoard;
using Vtodo.UseCases.Handlers.Boards.Dto;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Boards.Commands
{
    public class DeleteBoardRequestHandlerTest
    {
         private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulDeleteBoard_ReturnsTask()
        {
            SetupDbContext();
            
            var request = new DeleteBoardRequest() { Id = 1 };

            var deleteBoardRequestHandler = new DeleteBoardRequestHandler(_dbContext, SetupProjectSecurityService().Object);
            
            Assert.NotNull(_dbContext.Boards.FirstOrDefault(x => x.Id == 1));
            
            deleteBoardRequestHandler.Handle(request, CancellationToken.None);
            
            Assert.Null(_dbContext.Boards.FirstOrDefault(x => x.Id == 1));
            CleanUp();
        }
        
        [Fact]
        public async void Handle_BoardNotFound_ThrowsBoardNotFoundException()
        {
            SetupDbContext();

            var request = new DeleteBoardRequest() { Id = 2 };

            var deleteBoardRequestHandler = new DeleteBoardRequestHandler(_dbContext, SetupProjectSecurityService().Object);
            
            await Assert.ThrowsAsync<BoardNotFoundException>(() => deleteBoardRequestHandler.Handle(request, CancellationToken.None));
       
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