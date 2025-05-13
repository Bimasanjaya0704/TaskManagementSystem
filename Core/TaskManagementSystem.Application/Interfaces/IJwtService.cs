using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.Interfaces;

public interface IJwtService
{
    public string GenerateToken(UserDTO user);
    bool ValidateToken(string token, out Guid userId);
    Role GetRoleFromToken(string token);
}