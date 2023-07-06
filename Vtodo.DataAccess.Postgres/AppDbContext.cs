using System;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Vtodo.DataAccess.Postgres
{
    internal class AppDbContext : DbContext, IDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<Board> Boards { get; set; } = null!;
        public DbSet<TaskM> Tasks { get; set; } = null!;
        public DbSet<ProjectAccountsRoles> ProjectAccountsRoles { get; set; } = null!;
        public DbSet<ProjectFile> ProjectFiles { get; set; } = null!;
        public DbSet<ProjectBoardFile> ProjectBoardsFiles { get; set; } = null!;
        public DbSet<ProjectTaskFile> ProjectTasksFiles { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectAccountsRoles>()
                .HasKey(m => new { m.ProjectId , m.AccountId, m.ProjectRole });
            
            modelBuilder.Entity<ProjectFile>()
                .HasKey(m => new { m.ProjectId , m.FileName });
            
            modelBuilder.Entity<ProjectBoardFile>()
                .HasKey(m => new { m.ProjectId, m.BoardId , m.FileName });
            
            modelBuilder.Entity<ProjectTaskFile>()
                .HasKey(m => new { m.ProjectId, m.TaskId , m.FileName });
        }
    }
}