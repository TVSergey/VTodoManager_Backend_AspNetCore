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
using Vtodo.UseCases.Handlers.Boards.Commands.CreateBoard;
using Vtodo.UseCases.Handlers.Boards.Dto;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Boards.Commands
{
    public class CreateBoarRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulCreateBoard_ReturnsTask()
        {
            SetupDbContext();

            var createBoardDto = new CreateBoardDto() { Title ="Test Board", ProjectId = 1, PrioritySort = 0};
            var request = new CreateBoardRequest() {CreateBoardDto = createBoardDto};
            
            var mockMapper = SetupMockMapper();
            mockMapper.Setup(x => x.Map<Board>(createBoardDto)).Returns(new Board() {Title = createBoardDto.Title, PrioritySort = 0});
            
            var createBoardRequestHandler = new CreateBoardRequestHandler(_dbContext, SetupProjectSecurityService().Object, mockMapper.Object);
            
            createBoardRequestHandler.Handle(request, CancellationToken.None);
            
            Assert.NotNull(_dbContext.Boards.FirstOrDefault(x => x.Title == createBoardDto.Title));
            CleanUp();
        }
        
        [Fact]
        public async void Handle_ProjectNotFound_ThrowsProjectNotFoundException()
        {
            SetupDbContext();

            var createBoardDto = new CreateBoardDto() { Title ="Test Board", ProjectId = 2, PrioritySort = 0};
            var request = new CreateBoardRequest() {CreateBoardDto = createBoardDto};
            
            var mockMapper = SetupMockMapper();
            mockMapper.Setup(x => x.Map<Board>(createBoardDto)).Returns(new Board() {Title = createBoardDto.Title, PrioritySort = 0});
            
            var createBoardRequestHandler = new CreateBoardRequestHandler(_dbContext, SetupProjectSecurityService().Object, mockMapper.Object);
            
            await Assert.ThrowsAsync<ProjectNotFoundException>(() => createBoardRequestHandler.Handle(request, CancellationToken.None));
       
            CleanUp();
        }
        
        private static Mock<IProjectSecurityService> SetupProjectSecurityService()
        {
            var mock = new Mock<IProjectSecurityService>();
            mock.Setup(x => x.CheckAccess(It.IsAny<Project>(), It.IsAny<ProjectRoles>())).Verifiable();
            
            return new Mock<IProjectSecurityService>();
        }
        
        private static Mock<IMapper> SetupMockMapper()
        {
            return new Mock<IMapper>();
        }
        
        private void SetupDbContext()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();
            
            _dbContext.Projects.Add(new Project() {Title = "Test Project"});
            _dbContext.SaveChanges();
        }
        
        private void CleanUp()
        {
            _dbContext?.Database.EnsureDeleted();
            _dbContext?.Dispose();
        }
    }
}