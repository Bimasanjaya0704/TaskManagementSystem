using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Domain.Entities;

[Table("ProjectMembers")]
public class ProjectMemberEntity
{
    [Key] 
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }

    public ProjectRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    
    // Navigation properties
    public virtual ProjectEntity Project { get; set; }
    public virtual UserEntity User { get; set; }
    
    public ProjectMemberEntity()
    {
        Role = ProjectRole.Member;
        JoinedAt = DateTime.UtcNow;
    }
}