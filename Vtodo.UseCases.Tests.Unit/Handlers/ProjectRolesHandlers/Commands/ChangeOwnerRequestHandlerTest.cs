using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Vtodo.UseCases.Handlers.ProjectsRoles.Commands.ChangeOwner;
using Vtodo.UseCases.Handlers.ProjectsRoles.Dto;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.ProjectRolesHandlers.Commands
{
    public class ChangeOwnerRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulChangeOwner_ReturnsTask()
        {
            SetupDbContext();

            var changeOwnerDto = new ChangeOwnerDto() { AccountId = 2};
            
            var request = new ChangeOwnerRequest() { ProjectId = 1, ChangeOwnerDto = changeOwnerDto};
            var changeOwnerRequest = new ChangeOwnerRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            changeOwnerRequest.Handle(request, CancellationToken.None);
            
            Assert.NotNull(_dbContext.ProjectAccountsRoles.FirstOrDefault(x => 
                x.AccountId == changeOwnerDto.AccountId && 
                x.ProjectId == request.ProjectId && 
                x.ProjectRole == ProjectRoles.ProjectOwner));
          
            CleanUp();
        }

        [Fact]
        public async void Handle_ProjectNotFound_ThrowsProjectNotFoundException()
        {
            SetupDbContext();
            
            var changeOwnerDto = new ChangeOwnerDto() { AccountId = 2};
            
            var request = new ChangeOwnerRequest() { ProjectId = 4, ChangeOwnerDto = changeOwnerDto};
            var changeOwnerRequest = new ChangeOwnerRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);
            
            await Assert.ThrowsAsync<ProjectNotFoundException>(() => changeOwnerRequest.Handle(request, CancellationToken.None));
            
            CleanUp();
        }

        [Fact]
        public async void Handle_AccountNotFound_ThrowsAccountNotFoundException()
        {
            SetupDbContext();
            
            var changeOwnerDto = new ChangeOwnerDto() { AccountId = 5};
            
            var request = new ChangeOwnerRequest() { ProjectId = 1, ChangeOwnerDto = changeOwnerDto};
            var changeOwnerRequest = new ChangeOwnerRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);
            
            await Assert.ThrowsAsync<AccountNotFoundException>(() => changeOwnerRequest.Handle(request, CancellationToken.None));
            
            CleanUp();
        }

        private Mock<IProjectSecurityService> SetupProjectSecurityServiceMock()
        {
            var mock = new Mock<IProjectSecurityService>();
            mock.Setup(x => x.CheckAccess(It.IsAny<Project>(), It.IsAny<ProjectRoles>())).Verifiable();
            mock.Setup(x => x.ChangeOwner(It.IsAny<Project>(), It.IsAny<Account>())).Callback((Project project, Account account) =>
            {
                _dbContext.ProjectAccountsRoles.Remove(
                    _dbContext.ProjectAccountsRoles.First(x => x.ProjectRole == ProjectRoles.ProjectOwner));
                _dbContext.ProjectAccountsRoles.Add(new ProjectAccountsRoles() {ProjectId = project.Id, AccountId = account.Id, ProjectRole = ProjectRoles.ProjectOwner});
                _dbContext.SaveChanges();
            });
            
            return mock;
        }
        
        private void SetupDbContext()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();

            _dbContext.Accounts.Add(new Account() { Email = "test@test.ru", Username = "test", HashedPassword = "test" , Salt = new byte[64]});
            _dbContext.Accounts.Add(new Account() { Email = "test2@test.ru", Username = "test2", HashedPassword = "test2" , Salt = new byte[64]});
            
            _dbContext.Projects.Add(new Project() {Title = "Test Project"});
            _dbContext.SaveChanges();

            _dbContext.ProjectAccountsRoles.AddRange(new List<ProjectAccountsRoles>()
            {
                new ProjectAccountsRoles()
            {
                AccountId = _dbContext.Accounts.First(x => x.Id == 1).Id, 
                ProjectId  = _dbContext.Projects.First(x => x.Id == 1).Id, 
                ProjectRole = ProjectRoles.ProjectOwner
            },
                new ProjectAccountsRoles()
                {
                    AccountId = _dbContext.Accounts.First(x => x.Id == 2).Id, 
                    ProjectId  = _dbContext.Projects.First(x => x.Id == 1).Id, 
                    ProjectRole = ProjectRoles.ProjectMember
                }
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