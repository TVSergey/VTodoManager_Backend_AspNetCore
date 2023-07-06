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
using Vtodo.UseCases.Handlers.ProjectsRoles.Commands.RevokeAllRole;
using Vtodo.UseCases.Handlers.ProjectsRoles.Dto;
using Xunit;

namespace Vtodo.UseCases.Tests.Unit.Handlers.ProjectRolesHandlers.Commands
{
    public class KickMemberRequestHandlerTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void Handle_SuccessfulKickMember_ReturnsTask()
        {
            SetupDbContext();

            var kickMemberDto = new KickMemberDto() { AccountId = 2};
            var request = new KickMemberRequest() { ProjectId = 1, KickMemberDto = kickMemberDto};

            var kickMemberRequestHandler = new KickMemberRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            kickMemberRequestHandler.Handle(request, CancellationToken.None);

            Assert.Empty(_dbContext.ProjectAccountsRoles.Where(x => x.AccountId == kickMemberDto.AccountId && x.ProjectId == kickMemberDto.AccountId));
            
            CleanUp();
        }

        [Fact]
        public async void Handle_ProjectNotFound_ThrowsProjectNotFoundException()
        {
            SetupDbContext();
            
            var kickMemberDto = new KickMemberDto() { AccountId = 2};
            var request = new KickMemberRequest() { ProjectId = 10, KickMemberDto = kickMemberDto};

            var kickMemberRequestHandler = new KickMemberRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            await Assert.ThrowsAsync<ProjectNotFoundException>(() => kickMemberRequestHandler.Handle(request, CancellationToken.None));

            CleanUp();
        }
        
        [Fact]
        public async void Handle_AccountNotFound_ThrowsAccountNotFoundException()
        {
            SetupDbContext();
            
            var kickMemberDto = new KickMemberDto() { AccountId = 20};
            var request = new KickMemberRequest() { ProjectId = 1, KickMemberDto = kickMemberDto};

            var kickMemberRequestHandler = new KickMemberRequestHandler(_dbContext, SetupProjectSecurityServiceMock().Object);

            await Assert.ThrowsAsync<AccountNotFoundException>(() => kickMemberRequestHandler.Handle(request, CancellationToken.None));

            CleanUp();
        }

        private void SetupDbContext()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();

            _dbContext.Accounts.Add(new Account() { Email = "test@test.ru", Username = "test", HashedPassword = "test" , Salt = new byte[64]});
            _dbContext.Accounts.Add(new Account() { Email = "test2@test.ru", Username = "test2", HashedPassword = "test2" , Salt = new byte[64]});
            _dbContext.Accounts.Add(new Account() { Email = "test3@test.ru", Username = "test3", HashedPassword = "test3" , Salt = new byte[64]});
            _dbContext.Accounts.Add(new Account() { Email = "test4@test.ru", Username = "test4", HashedPassword = "test4" , Salt = new byte[64]});
            
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
                    AccountId = _dbContext.Accounts.First(x => x.Id == 1).Id, 
                    ProjectId  = _dbContext.Projects.First(x => x.Id == 1).Id, 
                    ProjectRole = ProjectRoles.ProjectMember
                },
                new ProjectAccountsRoles()
                {
                    AccountId = _dbContext.Accounts.First(x => x.Id == 2).Id, 
                    ProjectId  = _dbContext.Projects.First(x => x.Id == 1).Id, 
                    ProjectRole = ProjectRoles.ProjectUpdate
                },
                new ProjectAccountsRoles()
                {
                    AccountId = _dbContext.Accounts.First(x => x.Id == 2).Id, 
                    ProjectId  = _dbContext.Projects.First(x => x.Id == 1).Id, 
                    ProjectRole = ProjectRoles.ProjectMember
                }, new ProjectAccountsRoles()
                {
                    AccountId = _dbContext.Accounts.First(x => x.Id == 3).Id, 
                    ProjectId  = _dbContext.Projects.First(x => x.Id == 1).Id, 
                    ProjectRole = ProjectRoles.ProjectMember
                }
            });
            _dbContext.SaveChanges();
        }
        
        private Mock<IProjectSecurityService> SetupProjectSecurityServiceMock()
        {
            var mock = new Mock<IProjectSecurityService>();
            mock.Setup(x => x.CheckAccess(It.IsAny<Project>(), It.IsAny<ProjectRoles>())).Verifiable();
            mock.Setup(x => x.RevokeAllRoles(It.IsAny<Project>(), It.IsAny<Account>()))
                .Callback((Project project, Account account) =>
                {
                    _dbContext.ProjectAccountsRoles.RemoveRange(_dbContext.ProjectAccountsRoles
                        .Where(x => x.AccountId == account.Id && x.Project.Id == project.Id)
                        .ToList());
                    _dbContext.SaveChanges();
                });
            
            return mock;
        }
        
        private void CleanUp()
        {
            _dbContext?.Database.EnsureDeleted();
            _dbContext?.Dispose();
        }
    }
}