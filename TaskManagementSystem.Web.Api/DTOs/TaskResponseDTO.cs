using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Web.Api.DTOs;

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public TaskProgressStatus Status { get; set; }
    public int AssignedToUserId { get; set; }
    public int? ReviewedByUserId { get; set; }
    public int ProjectId { get; set; }
}