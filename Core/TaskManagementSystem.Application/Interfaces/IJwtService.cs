using TaskManagementSystem.Application.DTOs;

namespace TaskManagementSystem.Application.Interfaces;

public interface IJwtService
{
    public string GenerateToken(UserDTO user);
    bool ValidateToken(string token, out Guid userId);
}