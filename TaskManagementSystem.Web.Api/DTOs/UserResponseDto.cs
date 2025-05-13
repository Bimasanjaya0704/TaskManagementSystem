using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Web.Api.DTOs;

public class UserResponseDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; }
    public string? Email { get; set; }
    public Role Role { get; set; }
}