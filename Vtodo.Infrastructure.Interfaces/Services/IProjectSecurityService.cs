using Vtodo.Entities.Enums;
using Vtodo.Entities.Models;

namespace Vtodo.Infrastructure.Interfaces.Services
{
    internal interface IProjectSecurityService
    {
        void CheckAccess(Project project, ProjectRoles neededRole);
        void ChangeOwner(Project project, Account newOwner);
        void AddMember(Project project, Account newMember);
        void GrantRole(Project project, Account account, ProjectRoles newRole);
        void RevokeRole(Project project, Account account, ProjectRoles role);
        void RevokeAllRoles(Project project, Account account);

    }
}