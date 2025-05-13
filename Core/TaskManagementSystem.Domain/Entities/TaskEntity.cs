using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Domain.Entities;

[Table("Tasks")]
public class TaskEntity
{
    [Key]
    public Guid TaskId { get; set; } = Guid.NewGuid();
    
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskProgressStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public Guid? ReviewedToUserId { get; set; }
    
    // Navigation properties
    public virtual ProjectEntity Project { get; set; }
    public virtual UserEntity AssignedTo { get; set; }
    public virtual UserEntity ReviewedTo { get; set; }
    
    public TaskEntity()
    {
        Status = TaskProgressStatus.New;
        CreatedAt = DateTime.UtcNow;
    }
}