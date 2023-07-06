using System;
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
using Vtodo.UseCases.Handlers.Projects.Dto;
using Vtodo.UseCases.Handlers.Projects.Queries.GetProject;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Projects.Queries
{
    public class GetProjectRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulGetProject_ReturnsTaskProjectDto()
        {
            SetupDbContext();

            var request = new GetProjectRequest() { Id = 1 };

            var mapperMock = SetupMapperMock();
            mapperMock.Setup(x => x.Map<ProjectDto>(It.IsAny<Project>())).Returns(new ProjectDto()
            {
                Title = _dbContext.Projects.First(x => x.Id == request.Id).Title,
                CreationDate = new DateTimeOffset(_dbContext.Projects.First(x => x.Id == request.Id).CreationDate).ToUnixTimeMilliseconds()
            });

            var getProjectRequestHandler = new GetProjectRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object, mapperMock.Object);

            var result = getProjectRequestHandler.Handle(request, CancellationToken.None).Result;
            
            Assert.NotNull(result);

            CleanUp();
        }

        [Fact]
        public async void Handle_ProjectNotFound_ThrowsProjectNotFoundException()
        {
            SetupDbContext();
            
            var request = new GetProjectRequest() { Id = 10 };

            var getProjectRequestHandler = new GetProjectRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object, SetupMapperMock().Object);

            await Assert.ThrowsAsync<ProjectNotFoundException>(() => getProjectRequestHandler.Handle(request, CancellationToken.None));

            CleanUp();
        }

        private static Mock<IMapper> SetupMapperMock()
        {
            return new Mock<IMapper>();
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