using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Result;

namespace TaskManagementSystem.Application.Interfaces;

public interface IAuthService
{
    Task<TaskErrorResult<UserDTO>> RegisterAsync(CreateUserDto createUserDto);
    Task<TaskErrorResult<AuthResponseDTO>> LoginAsync(LoginDTO loginDto);
}