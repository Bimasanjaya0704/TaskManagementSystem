using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Domain.Entities;

namespace Infrastructure.Presistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<ProjectMemberEntity> ProjectMembers { get; set; }
    public DbSet<FriendRequestEntity> FriendRequest { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.AssignedTo)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.ReviewedTo)
            .WithMany(u => u.ReviewedTasks)
            .HasForeignKey(t => t.ReviewedToUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.Project) 
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)  
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<FriendRequestEntity>()
            .HasOne(fr => fr.Sender)
            .WithMany(u => u.FriendshipsRequested)  
            .HasForeignKey(fr => fr.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
    
        modelBuilder.Entity<FriendRequestEntity>()
            .HasOne(fr => fr.Receiver)
            .WithMany(u => u.FriendshipsReceived)  
            .HasForeignKey(fr => fr.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        
        base.OnModelCreating(modelBuilder);
    }

}