using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.DTOs;

public class UserDTO
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
}