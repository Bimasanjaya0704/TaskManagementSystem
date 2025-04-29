using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Domain.Entities;

public class ProjectEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
    public ProjectPriority ProjectPriority { get; set; }
    public virtual UserEntity CreatedByUser { get; set; }
    public int CreatedByUserId { get; set; }
    public virtual List<TaskEntity> Tasks { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ProjectEntity()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProjectDate()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}