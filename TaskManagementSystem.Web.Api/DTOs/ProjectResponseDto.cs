using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Web.Api.DTOs;

public class ProjectResponseDto
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
    public ProjectPriority ProjectPriority { get; set; }

    public Guid CreatorUserId { get; set; }
    public List<TaskResponseDto> Tasks { get; set; } = new();

    public DateTime CreatedAt { get; set; }
}