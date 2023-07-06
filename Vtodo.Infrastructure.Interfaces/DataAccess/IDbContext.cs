using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vtodo.Entities.Models;

namespace Vtodo.Infrastructure.Interfaces.DataAccess
{
    internal interface IDbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<TaskM> Tasks { get; set; }
        public DbSet<ProjectAccountsRoles> ProjectAccountsRoles { get; set; }
        public DbSet<ProjectFile> ProjectFiles { get; set; }
        public DbSet<ProjectBoardFile> ProjectBoardsFiles { get; set; }
        public DbSet<ProjectTaskFile> ProjectTasksFiles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        
        int SaveChanges();
    }
}