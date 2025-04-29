using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Web.Api.DTOs;

public class ProjectRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
    public ProjectPriority ProjectPriority { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}