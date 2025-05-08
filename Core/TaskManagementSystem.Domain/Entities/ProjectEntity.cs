using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Domain.Entities;

[Table("Projects")]
public class ProjectEntity
{
    [Key]
    public Guid ProjectId { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatorUserId { get; set; }
    public DateTime DueDate { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
    public ProjectPriority ProjectPriority { get; set; }

    // Navigation properties
    public virtual UserEntity Creator { get; set; }
    public virtual ICollection<TaskEntity> Tasks { get; set; }
    public virtual ICollection<ProjectMemberEntity> Members { get; set; }
    
    public ProjectEntity()
    {
        Tasks = new HashSet<TaskEntity>();
        Members = new HashSet<ProjectMemberEntity>();
        CreatedAt = DateTime.UtcNow;
    }
}