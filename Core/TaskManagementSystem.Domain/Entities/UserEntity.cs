using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Domain.Entities;

[Table("Users")]
public class UserEntity
{
    [Key]
    public Guid UserId { get; set; }
    [Required]
    public string FirstName { get; set; } 
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string PasswordHash { get; set; } // In production, use proper password hashing
    public Role Role { get; set; } = Role.User;
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<ProjectMemberEntity> ProjectMemberships { get; set; }
    public virtual ICollection<TaskEntity> AssignedTasks { get; set; }
    public virtual ICollection<TaskEntity> ReviewedTasks { get; set; }
    public virtual ICollection<FriendRequestEntity> FriendshipsRequested { get; set; } = new List<FriendRequestEntity>();
    public virtual ICollection<FriendRequestEntity> FriendshipsReceived { get; set; } = new List<FriendRequestEntity>();
    
    public UserEntity()
    {
        ProjectMemberships = new HashSet<ProjectMemberEntity>();
        AssignedTasks = new HashSet<TaskEntity>();
        FriendshipsRequested = new HashSet<FriendRequestEntity>();
        FriendshipsReceived = new HashSet<FriendRequestEntity>();
        CreatedAt = DateTime.UtcNow;
    }
}