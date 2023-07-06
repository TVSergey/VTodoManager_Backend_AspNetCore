using System.Linq;
using System.Threading;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Vtodo.UseCases.Handlers.Projects.Commands.UpdateProject;
using Vtodo.UseCases.Handlers.Projects.Dto;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Projects.Commands
{
    public class UpdateProjectRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;
        
        [Fact]
        public void Handle_SuccessfulUpdateProject_ReturnsTask()
        {
            SetupDbContext();

            var updateProjectDto = new UpdateProjectDto() { Title = "Test update project"};
            
            var request = new UpdateProjectRequest() { Id = 1, UpdateProjectDto = updateProjectDto};

            var updateProjectRequestHandler = new UpdateProjectRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            updateProjectRequestHandler.Handle(request, CancellationToken.None);
            
            Assert.NotNull(_dbContext.Projects.FirstOrDefault(x => x.Id == request.Id && x.Title == updateProjectDto.Title));
            
            CleanUp();
        }

        [Fact]
        public async void Handle_ProjectNotFound_ThrowsProjectNotFoundException()
        {
            SetupDbContext();

            var updateProjectDto = new UpdateProjectDto() { Title = "Test update project" };
            
            var request = new UpdateProjectRequest() { Id = 2, UpdateProjectDto = updateProjectDto };

            var updateProjectRequestHandler = new UpdateProjectRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            await Assert.ThrowsAsync<ProjectNotFoundException>(() => updateProjectRequestHandler.Handle(request, CancellationToken.None));

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
        }
        
        private void CleanUp()
        {
            _dbContext?.Database.EnsureDeleted();
            _dbContext?.Dispose();
        }
    }
}