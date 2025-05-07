using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Result;

namespace TaskManagementSystem.Application.Interfaces;

public interface IUserService
{
    Task<TaskErrorResult<UserDTO>> GetUserByIdAsync(Guid id, string token);
    Task<TaskErrorResult<IEnumerable<UserDTO>>> GetAllUsersAsync(string token);
    Task<TaskErrorResult<UserDTO>> CreateUserAsync(CreateUserDtoAdmin createUserDto, string token);
    Task<TaskErrorResult<UserDTO>> UpdateUserAsync(Guid id, string token, UpdateUserDto userDto);
    Task<TaskErrorResult<UserDTO>> DeleteUserAsync(Guid id, string token);
    Task<TaskErrorResult<UserDTO>> GetUserByEmailAsync(string email, string token);
    Task<TaskErrorResult<UserDTO>> GetUserByUsernameAsync(string username, string token);
}