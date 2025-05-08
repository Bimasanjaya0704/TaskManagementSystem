using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Web.Api.DTOs;

public class TaskRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public TaskProgressStatus Status { get; set; } = TaskProgressStatus.New;
    public Guid AssignedToUserId { get; set; }
    public Guid? ReviewedToUserId { get; set; }
    public Guid ProjectId { get; set; }
}