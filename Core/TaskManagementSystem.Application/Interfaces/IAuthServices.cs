using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Result;

namespace TaskManagementSystem.Application.Interfaces;

public interface IAuthService
{
    Task<TaskErrorResult<UserDTO>> RegisterAsync(RegisterDTO registerDto);
    Task<TaskErrorResult<AuthResponseDTO>> LoginAsync(LoginDTO loginDto);
}