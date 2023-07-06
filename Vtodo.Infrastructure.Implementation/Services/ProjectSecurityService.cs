using System;
using System.Linq;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.Infrastructure.Implementation.Services
{
    internal class ProjectSecurityService : IProjectSecurityService
    {
        private readonly IDbContext _dbContext;
        private readonly ICurrentAccountService _currentAccountService;
        
        public ProjectSecurityService(IDbContext dbContext, ICurrentAccountService currentAccountService)
        {
            _dbContext = dbContext;
            _currentAccountService = currentAccountService;
        }

        public void CheckAccess(Project project, ProjectRoles neededRole)
        {
            var account = _currentAccountService.Account;
            var roles = _dbContext.ProjectAccountsRoles.Where(x => x.Project.Id == project.Id && x.Account.Id == account.Id).ToList();

            if (roles.Find(x => x.ProjectRole == ProjectRoles.ProjectOwner) != null) return;
            if (neededRole == ProjectRoles.ProjectOwner) throw new AccessDeniedException();
            
            if (roles.Find(x => x.ProjectRole == ProjectRoles.ProjectAdmin) != null) return;

            if (roles.Find(x => x.ProjectRole == neededRole) == null) throw new AccessDeniedException();
        }

        public void ChangeOwner(Project project, Account newOwner)
        {
            var oldOwner = _dbContext.ProjectAccountsRoles.FirstOrDefault(x =>
                    x.Project.Id == project.Id && x.ProjectRole == ProjectRoles.ProjectOwner);
            
            var projectRoles = _dbContext.ProjectAccountsRoles.Where(x => 
                x.Project.Id == project.Id &&
                x.Account.Id == newOwner.Id).ToList();

            if (oldOwner!= null)
            {
                CheckAccess(project, ProjectRoles.ProjectOwner);
                
                if (projectRoles.Find(x => x.ProjectRole == ProjectRoles.ProjectMember) == null) throw new AccountNotMemberInProjectException();
                
                if (_dbContext.ProjectAccountsRoles.FirstOrDefault(x =>
                        x.Project.Id == project.Id && 
                        x.ProjectRole == ProjectRoles.ProjectMember && 
                        x.Account.Id == oldOwner.Account.Id) == null)
                    AddMember(project, oldOwner.Account);
                
                _dbContext.ProjectAccountsRoles.Remove(oldOwner);
            }

            _dbContext.ProjectAccountsRoles.Add(new ProjectAccountsRoles() { Project = project,Account = newOwner, ProjectRole = ProjectRoles.ProjectOwner}); 
            _dbContext.SaveChanges();
        }

        
        public void AddMember(Project project, Account newMember)
        {
            CheckAccess(project, ProjectRoles.ProjectAdmin);
            var member = _dbContext.ProjectAccountsRoles.FirstOrDefault(x =>
                x.Project.Id == project.Id && 
                x.Account.Id == newMember.Id && 
                x.ProjectRole == ProjectRoles.ProjectMember);
            if (member != null) throw new ProjectRolesAlreadyExistsException();
            
            _dbContext.ProjectAccountsRoles.Add(new ProjectAccountsRoles() { Project = project,Account = newMember, ProjectRole = ProjectRoles.ProjectMember}); 
            _dbContext.SaveChanges();
        }
        
        public void GrantRole(Project project, Account account, ProjectRoles newRole)
        {
            if (newRole == ProjectRoles.ProjectOwner) throw new AttemptChangeOwnerFromGrantRoleException();
            if (newRole == ProjectRoles.ProjectMember) throw new AttemptAddMemberFromGrantRoleException();
            CheckAccess(project, ProjectRoles.ProjectAdmin);
            
            var projectRole = _dbContext.ProjectAccountsRoles.Where(x => 
                x.Project.Id == project.Id &&
                x.Account.Id == account.Id).ToList();

            if (projectRole.Find(x => x.ProjectRole == ProjectRoles.ProjectMember) == null) throw new AccountNotMemberInProjectException();
            if (projectRole.Find(x => x.ProjectRole == newRole) != null) throw new ProjectRolesAlreadyExistsException();
            
            _dbContext.ProjectAccountsRoles.Add(new ProjectAccountsRoles() { Project = project,Account = account, ProjectRole = newRole});
            _dbContext.SaveChanges();
        }

        public void RevokeRole(Project project, Account account, ProjectRoles role)
        {
            if (role == ProjectRoles.ProjectOwner) throw new AttemptToRemoveOwnerRoleException();
            CheckAccess(project, ProjectRoles.ProjectAdmin);

            if (role == ProjectRoles.ProjectMember)
            {
                RevokeAllRoles(project, account); //TODO заменить на исключение?
                return;
            }
            
            var projectRole = _dbContext.ProjectAccountsRoles.FirstOrDefault(x => 
                x.Project.Id == project.Id && 
                x.Account.Id == account.Id &&
                x.ProjectRole == role);
            if (projectRole == null) throw new ProjectRoleNotFoundException("Account with this role not found in project");
            
            _dbContext.ProjectAccountsRoles.Remove(projectRole);
            _dbContext.SaveChanges();
        }

        public void RevokeAllRoles(Project project, Account account)
        {
            CheckAccess(project, ProjectRoles.ProjectAdmin);
            
            var projectRoles = _dbContext.ProjectAccountsRoles.Where(x => 
                x.Project.Id == project.Id && 
                x.Account.Id == account.Id).ToList();
            
            if(projectRoles.Count == 0) throw new AccountNotMemberInProjectException();
            if(projectRoles.FirstOrDefault(x => x.ProjectRole == ProjectRoles.ProjectOwner) != null) throw new AttemptToRemoveOwnerRoleException();
            
            _dbContext.ProjectAccountsRoles.RemoveRange(projectRoles);
            _dbContext.SaveChanges();
        }
    }
}