using System;
using System.Collections.Generic;
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
using Vtodo.UseCases.Handlers.Projects.Queries.GetUserProjectList;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.Projects.Queries
{
    public class GetAccountProjectsListRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulGetAccountProjectsList_ReturnsTaskListProjectDto()
        {
            SetupDbContext();

            var request = new GetAccountProjectsListRequest() { UserId = 1 };

            var project1 = _dbContext.Projects.First(x => x.Id == 1);
            var project2 = _dbContext.Projects.First(x => x.Id == 3);

            var mapperMock = SetupMapperMock();
            mapperMock.Setup(x => x.Map<List<ProjectDto>>(It.IsAny<List<Project>>())).Returns(new List<ProjectDto>()
            {
                new ProjectDto() { Id = project1.Id, Title = project1.Title, CreationDate = new DateTimeOffset(project1.CreationDate).ToUnixTimeMilliseconds() },
                new ProjectDto() { Id = project2.Id, Title = project2.Title, CreationDate = new DateTimeOffset(project2.CreationDate).ToUnixTimeMilliseconds() }
            });

            var getAccountProjectsListRequestHandler = new GetAccountProjectsListRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object, mapperMock.Object);

            var result = getAccountProjectsListRequestHandler.Handle(request, CancellationToken.None).Result;
            
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            CleanUp();
        }

        [Fact]
        public async void Handle_AccountNotFound_ThrowsAccountNotFoundException()
        {
            SetupDbContext();

            var request = new GetAccountProjectsListRequest() { UserId = 30 };

            var getAccountProjectsListRequestHandler = new GetAccountProjectsListRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object, SetupMapperMock().Object);

            await Assert.ThrowsAsync<AccountNotFoundException>(() => getAccountProjectsListRequestHandler.Handle(request, CancellationToken.None));

            CleanUp();
        }
        
        [Fact]
        public async void Handle_ProjectNotFound_ThrowsProjectNotFoundException()
        {
            SetupDbContext();

            var request = new GetAccountProjectsListRequest() {UserId = 3};

            var getAccountProjectsListRequestHandler = new GetAccountProjectsListRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object, SetupMapperMock().Object);

            await Assert.ThrowsAsync<ProjectNotFoundException>(() => getAccountProjectsListRequestHandler.Handle(request, CancellationToken.None));

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
            _dbContext.Accounts.Add(new Account() { Email = "test2@test.ru", Username = "test2", HashedPassword = "test2" , Salt = new byte[64]});
            _dbContext.Accounts.Add(new Account() { Email = "test3@test.ru", Username = "test3", HashedPassword = "test3" , Salt = new byte[64]});
            
            _dbContext.Projects.Add(new Project() {Title = "Test Project"});
            _dbContext.Projects.Add(new Project() {Title = "Test Project2"});
            _dbContext.Projects.Add(new Project() {Title = "Test Project3"});

            _dbContext.SaveChanges();

            _dbContext.ProjectAccountsRoles.Add(new ProjectAccountsRoles()
            {
                Project = _dbContext.Projects.First(x => x.Id == 1),
                Account = _dbContext.Accounts.First(x => x.Id == 1),
                ProjectRole = ProjectRoles.ProjectMember
            });
            
            _dbContext.ProjectAccountsRoles.Add(new ProjectAccountsRoles()
            {
                Project = _dbContext.Projects.First(x => x.Id == 1),
                Account = _dbContext.Accounts.First(x => x.Id == 1),
                ProjectRole = ProjectRoles.ProjectOwner
            });
            
            _dbContext.ProjectAccountsRoles.Add(new ProjectAccountsRoles()
            {
                Project = _dbContext.Projects.First(x => x.Id == 3),
                Account = _dbContext.Accounts.First(x => x.Id == 1),
                ProjectRole = ProjectRoles.ProjectMember
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