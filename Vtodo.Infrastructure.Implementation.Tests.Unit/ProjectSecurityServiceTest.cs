using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Implementation.Services;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Xunit;

namespace Vtodo.Infrastructure.Implementation.Tests.Unit
{
    public class ProjectSecurityServiceTest
    {
        private AppDbContext _dbContext = null!;
        
        [Fact]
        public void CheckAccess_RequiredRoleOwnerFound_ReturnsVoid()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);

            projectSecurityService.CheckAccess(_dbContext.Projects.First(x => x.Id == 1), ProjectRoles.ProjectOwner);
            CleanUp();
        }
        
        [Fact]
        public void CheckAccess_RequiredRoleFound_ReturnsVoid()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 2)).Object);

            projectSecurityService.CheckAccess(_dbContext.Projects.First(x => x.Id == 1), ProjectRoles.ProjectAdmin);
            CleanUp();
        }
        
        [Fact]
        public void CheckAccess_RequiredOwnerRoleNotFound_ThrowsAccessDeniedException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 2)).Object);

            Assert.Throws<AccessDeniedException>(() => projectSecurityService.CheckAccess(_dbContext.Projects.First(x => x.Id == 1), ProjectRoles.ProjectOwner));
            CleanUp();
        }

        [Fact]
        public void CheckAccess_RequiredRoleNotFound_ThrowsAccessDeniedException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 4)).Object);

            Assert.Throws<AccessDeniedException>(() => projectSecurityService.CheckAccess(_dbContext.Projects.First(x => x.Id == 1), ProjectRoles.ProjectAdmin));
            CleanUp();
        }

        [Fact]
        public void ChangeOwner_SuccessfulAddFirstOwnerWhenProjectCreated_ReturnsVoid()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 9)).Object);

            projectSecurityService.ChangeOwner(
                _dbContext.Projects.First(x => x.Id == 3),
                _dbContext.Accounts.First(x => x.Id == 9));

            Assert.NotNull(_dbContext.ProjectAccountsRoles.FirstOrDefault(
                x => x.Account.Id == 9
                     && x.Project.Id == 3
                     && x.ProjectRole == ProjectRoles.ProjectOwner));
            CleanUp();
        }
        
        [Fact]
        public void ChangeOwner_SuccessfulChangeOwner_ReturnsVoid()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);

           projectSecurityService.ChangeOwner(
                _dbContext.Projects.First(x => x.Id == 1),
                _dbContext.Accounts.First(x => x.Id == 4));
           
           Assert.NotNull(_dbContext.ProjectAccountsRoles.FirstOrDefault(
               x => x.Account.Id == 1 
                    && x.Project.Id == 1 
                    && x.ProjectRole == ProjectRoles.ProjectMember));
           
           Assert.Null(_dbContext.ProjectAccountsRoles.FirstOrDefault(
               x => x.Account.Id == 1 
                    && x.Project.Id == 1 
                    && x.ProjectRole == ProjectRoles.ProjectOwner));
           
           Assert.NotNull(_dbContext.ProjectAccountsRoles.FirstOrDefault(
               x => x.Account.Id == 4
                    && x.Project.Id == 1 
                    && x.ProjectRole == ProjectRoles.ProjectOwner));
            CleanUp();
        }

        
        [Fact]
        public void ChangeOwner_AccountNotMember_ThrowsAccountNotMemberInProjectException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            Assert.Throws<AccountNotMemberInProjectException>(() => projectSecurityService.ChangeOwner(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Accounts.First(x => x.Id == 8)));
            CleanUp();
        }


        [Fact]
        public void AddMember_SuccessfulAddMember_ReturnsVoid()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            projectSecurityService.AddMember(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Accounts.First(x => x.Id == 9));
            
            Assert.NotNull(_dbContext.ProjectAccountsRoles.FirstOrDefault(x =>
                x.Project == _dbContext.Projects.First(d => d.Id == 1) && 
                x.Account == _dbContext.Accounts.First(d => d.Id == 9) &&
                x.ProjectRole == ProjectRoles.ProjectMember));

            CleanUp();
        }
        
        [Fact]
        public void AddMember_AlreadyMember_ThrowsProjectRolesAlreadyExistsException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);

            Assert.Throws<ProjectRolesAlreadyExistsException>(() => projectSecurityService.AddMember(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Accounts.First(x => x.Id == 4)));
            
            CleanUp();
        }

        [Fact]
        public void GrantRole_SuccessfulGrantRole_ReturnsVoid()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            projectSecurityService.GrantRole(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Accounts.First(x => x.Id == 4), 
                ProjectRoles.ProjectUpdate);
            
            Assert.NotNull(_dbContext.ProjectAccountsRoles.FirstOrDefault(x =>
                x.Project == _dbContext.Projects.First(d => d.Id == 1) && 
                x.Account == _dbContext.Accounts.First(d => d.Id == 4) &&
                x.ProjectRole == ProjectRoles.ProjectUpdate));
            
            CleanUp();
        }
        
        [Fact]
        public void GrantRole_AttemptChangeOwnerFromGrantRole_ThrowsAttemptChangeOwnerFromGrantRoleException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            Assert.Throws<AttemptChangeOwnerFromGrantRoleException>(() => projectSecurityService.GrantRole(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Accounts.First(x => x.Id == 4), 
                ProjectRoles.ProjectOwner));

            CleanUp();
        }
        
        [Fact]
        public void GrantRole_AttemptAddMemberFromGrantRole_ThrowsAttemptAddMemberFromGrantRoleException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            Assert.Throws<AttemptAddMemberFromGrantRoleException>(() => projectSecurityService.GrantRole(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Accounts.First(x => x.Id == 9), 
                ProjectRoles.ProjectMember));

            CleanUp();
        }
        
        [Fact]
        public void GrantRole_AccountIsNotAMember_ThrowsAccountNotMemberInProjectException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            Assert.Throws<AccountNotMemberInProjectException>(() => projectSecurityService.GrantRole(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Accounts.First(x => x.Id == 5), 
                ProjectRoles.ProjectUpdate));

            CleanUp();
        }
        
        [Fact]
        public void GrantRole_AccountAlreadyHasRole_ThrowsProjectRolesAlreadyExistsException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            Assert.Throws<ProjectRolesAlreadyExistsException>(() => projectSecurityService.GrantRole(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Accounts.First(x => x.Id == 3), 
                ProjectRoles.ProjectUpdate));

            CleanUp();
        }
        
        [Fact]
        public void RevokeRole_SuccessfulRevokeRole_ReturnsVoid()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            projectSecurityService.RevokeRole(
                _dbContext.Projects.First(x => x.Id == 1),
                _dbContext.Accounts.First(x => x.Id == 3),
                    ProjectRoles.ProjectUpdate
                );
            
            Assert.Null(_dbContext.ProjectAccountsRoles.FirstOrDefault(x =>
                x.Project == _dbContext.Projects.First(d => d.Id == 1) && 
                x.Account == _dbContext.Accounts.First(d => d.Id == 3) &&
                x.ProjectRole == ProjectRoles.ProjectUpdate));
            
            CleanUp();
        }
        
        [Fact]
        public void RevokeRole_SuccessfulRevokeRoleMember_ReturnsVoid()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            projectSecurityService.RevokeRole(
                _dbContext.Projects.First(x => x.Id == 1),
                _dbContext.Accounts.First(x => x.Id == 3),
                ProjectRoles.ProjectMember
            );
            
            Assert.Empty(_dbContext.ProjectAccountsRoles.Where(x =>
                x.Project == _dbContext.Projects.First(d => d.Id == 1) && 
                x.Account == _dbContext.Accounts.First(d => d.Id == 3)));
            
            CleanUp();
        }
        
        [Fact]
        public void RevokeRole_SuccessfulRevokeRoleMemberWithOwnerRole_ThrowsAttemptToRemoveOwnerRoleException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            Assert.Throws<AttemptToRemoveOwnerRoleException>(() => projectSecurityService.RevokeRole(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Accounts.First(x => x.Id == 1), 
                ProjectRoles.ProjectMember));
            
            CleanUp();
        }
        
        [Fact]
        public void RevokeRole_AttemptToRemoveOwnerRole_ThrowsAttemptToRemoveOwnerRoleException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);

            Assert.Throws<AttemptToRemoveOwnerRoleException>(() => projectSecurityService.RevokeRole(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Accounts.First(x => x.Id == 1), 
                ProjectRoles.ProjectOwner));
            
            CleanUp();
        }
        
        [Fact]
        public void RevokeRole_RoleNotFound_ThrowsProjectRoleNotFoundException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            Assert.Throws<ProjectRoleNotFoundException>(() => projectSecurityService.RevokeRole(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Accounts.First(x => x.Id == 3), 
                ProjectRoles.ProjectAdmin));
            
            CleanUp();
        }
        
        [Fact]
        public void RevokeAllRoles_SuccessfulRevokeAllRoles_ReturnsVoid()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            projectSecurityService.RevokeAllRoles(
                _dbContext.Projects.First(x => x.Id == 1),
                _dbContext.Accounts.First(x => x.Id == 3)
            );
            
            Assert.Empty(_dbContext.ProjectAccountsRoles.Where(x =>
                x.Project == _dbContext.Projects.First(d => d.Id == 1) && 
                x.Account == _dbContext.Accounts.First(d => d.Id == 3)));
            
            CleanUp();
        }
        
        [Fact]
        public void RevokeAllRoles_AccountNotMember_ThrowsAccountNotMemberInProjectException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            Assert.Throws<AccountNotMemberInProjectException>(() => projectSecurityService.RevokeAllRoles(
                _dbContext.Projects.First(x => x.Id == 1),
                _dbContext.Accounts.First(x => x.Id == 7)
            ));

            CleanUp();
        }
        
        [Fact]
        public void RevokeAllRoles_AttemptToRemoveOwnerRole_ThrowsAttemptToRemoveOwnerRoleException()
        {
            SetupDbContext();
            
            var projectSecurityService = new ProjectSecurityService(_dbContext, SetupMockCurrentAccountService(_dbContext.Accounts.First(x => x.Id == 1)).Object);
            
            Assert.Throws<AttemptToRemoveOwnerRoleException>(() => projectSecurityService.RevokeAllRoles(
                _dbContext.Projects.First(x => x.Id == 1),
                _dbContext.Accounts.First(x => x.Id == 1)
            ));

            CleanUp();
        }
        
        private void CleanUp()
        {
            _dbContext?.Database.EnsureDeleted();
            _dbContext?.Dispose();
        }

        private void SetupDbContext()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();
            
            _dbContext.Accounts.AddRange(GetTestAccountList());
            _dbContext.Projects.AddRange(GetTestProjectsList());
            _dbContext.ProjectAccountsRoles.AddRange(GetTestProjectsAccountRoles());
            _dbContext.SaveChanges();
        }
        
        private static IEnumerable<Account> GetTestAccountList()
        {
            return new List<Account>()
            {
                new Account() {Id = 1, Email = "test1@test.ru", Username = "test1", IsBanned = false, Salt = new byte[64]},
                new Account() {Id = 2, Email = "test2@test.ru", Username = "test2", IsBanned = false, Salt = new byte[64]},
                new Account() {Id = 3, Email = "test3@test.ru", Username = "test3", IsBanned = false, Salt = new byte[64]},
                new Account() {Id = 4, Email = "test4@test.ru", Username = "test4", IsBanned = false, Salt = new byte[64]},
                new Account() {Id = 5, Email = "test5@test.ru", Username = "test5", IsBanned = false, Salt = new byte[64]},
                new Account() {Id = 6, Email = "test6@test.ru", Username = "test6", IsBanned = false, Salt = new byte[64]},
                new Account() {Id = 7, Email = "test7@test.ru", Username = "test7", IsBanned = false, Salt = new byte[64]},
                new Account() {Id = 8, Email = "test8@test.ru", Username = "test8", IsBanned = false, Salt = new byte[64]},
                new Account() {Id = 9, Email = "test9@test.ru", Username = "test9", IsBanned = false, Salt = new byte[64]},
            };
        }
        
        private static IEnumerable<Project> GetTestProjectsList()
        {
            return new List<Project>()
            {
                new Project() {Id = 1, Title = "Test project1"},
                new Project() {Id = 2, Title = "Test project2"},
                new Project() {Id = 3, Title = "Test project3"},
            };
        }
        
        private static IEnumerable<ProjectAccountsRoles> GetTestProjectsAccountRoles()
        {
            return new List<ProjectAccountsRoles>()
            {
                new ProjectAccountsRoles() {AccountId = 1, ProjectId = 1, ProjectRole = ProjectRoles.ProjectOwner},
                new ProjectAccountsRoles() {AccountId = 2, ProjectId = 1, ProjectRole = ProjectRoles.ProjectAdmin},
                new ProjectAccountsRoles() {AccountId = 2, ProjectId = 1, ProjectRole = ProjectRoles.ProjectMember},
                new ProjectAccountsRoles() {AccountId = 3, ProjectId = 1, ProjectRole = ProjectRoles.ProjectUpdate},
                new ProjectAccountsRoles() {AccountId = 3, ProjectId = 1, ProjectRole = ProjectRoles.ProjectMember},
                new ProjectAccountsRoles() {AccountId = 4, ProjectId = 1, ProjectRole = ProjectRoles.ProjectMember},
                new ProjectAccountsRoles() {AccountId = 5, ProjectId = 2, ProjectRole = ProjectRoles.ProjectOwner},
                new ProjectAccountsRoles() {AccountId = 6, ProjectId = 2, ProjectRole = ProjectRoles.ProjectAdmin},
                new ProjectAccountsRoles() {AccountId = 6, ProjectId = 2, ProjectRole = ProjectRoles.ProjectMember},
                new ProjectAccountsRoles() {AccountId = 7, ProjectId = 2, ProjectRole = ProjectRoles.ProjectUpdate},
                new ProjectAccountsRoles() {AccountId = 7, ProjectId = 2, ProjectRole = ProjectRoles.ProjectMember},
                new ProjectAccountsRoles() {AccountId = 8, ProjectId = 2, ProjectRole = ProjectRoles.ProjectMember},
            };
        }
        
        private static Mock<ICurrentAccountService> SetupMockCurrentAccountService(Account account)
        {
            var currentAccountService = new Mock<ICurrentAccountService>();
            currentAccountService.Setup(x => x.Account).Returns(account);

            return currentAccountService;
        }
    }
}