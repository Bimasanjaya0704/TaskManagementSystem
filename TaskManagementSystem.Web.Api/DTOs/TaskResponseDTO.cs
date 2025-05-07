using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Web.Api.DTOs;

public class TaskResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public TaskProgressStatus Status { get; set; }
    public Guid AssignedToUserId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public Guid ProjectId { get; set; }
}