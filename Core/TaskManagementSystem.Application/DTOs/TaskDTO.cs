using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.DTOs;

public class TaskDTO
{
    public Guid TaskId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public TaskProgressStatus Status { get; set; }
    public Guid AssignedToUserId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public Guid ProjectId { get; set; }
}