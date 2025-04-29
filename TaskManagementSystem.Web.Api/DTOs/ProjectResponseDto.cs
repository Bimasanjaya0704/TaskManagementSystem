using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Web.Api.DTOs;

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
    public ProjectPriority ProjectPriority { get; set; }

    public int CreatedByUserId { get; set; }
    public List<TaskResponseDto> Tasks { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}