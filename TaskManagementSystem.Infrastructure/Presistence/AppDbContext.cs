using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Domain.Entities;

namespace Infrastructure.Presistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<ProjectEntity> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.AssignedToUser)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.ReviewedByUser)
            .WithMany(u => u.ReviewedTasks)
            .HasForeignKey(t => t.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.Project) 
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)  
            .OnDelete(DeleteBehavior.Cascade);
        
        base.OnModelCreating(modelBuilder);
    }

}