using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.DTOs;

public class ProjectDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
    public ProjectPriority ProjectPriority { get; set; }
    public int CreatedByUserId { get; set; }
    public List<TaskDTO> Tasks { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}