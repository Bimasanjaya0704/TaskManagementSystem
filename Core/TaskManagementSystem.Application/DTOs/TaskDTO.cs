using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.DTOs;

public class TaskDTO
{
    public Guid TaskId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public TaskProgressStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ProjectId { get; set; }
    public Guid AssignedToUserId { get; set; }
    public Guid? ReviewedToUserId { get; set; }
}