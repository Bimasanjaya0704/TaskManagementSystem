using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Web.Api.DTOs;

public class UserResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public Role Role { get; set; }

    
    public List<TaskDTO> AssignedTasks { get; set; } = new List<TaskDTO>();
    public List<TaskDTO> ReviewedTasks { get; set; } = new List<TaskDTO>();
    
}