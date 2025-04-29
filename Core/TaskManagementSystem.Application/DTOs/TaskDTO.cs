using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.DTOs;

public class TaskDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public TaskProgressStatus Status { get; set; }
    public int AssignedToUserId { get; set; }
    public int? ReviewedByUserId { get; set; }
    public int ProjectId { get; set; }
}