using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.DTOs;

public class AuthenticatedUserDTO
{
    public Guid UserId { get; set; }
    public Role Role { get; set; }
}