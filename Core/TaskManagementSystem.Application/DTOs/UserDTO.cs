using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.User;
    public string? Email { get; set; }
    public List<TaskDTO> AssignedTasks { get; set; } = new List<TaskDTO>();
    public List<TaskDTO> ReviewedTasks { get; set; } = new List<TaskDTO>();
}