using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Web.Api.DTOs;

public class AuthenticatedUser
{
    public Guid UserId { get; set; }
    public Role Role { get; set; }
}