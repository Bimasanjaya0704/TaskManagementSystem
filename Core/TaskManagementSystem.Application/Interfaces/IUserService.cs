using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Result;

namespace TaskManagementSystem.Application.Interfaces;

public interface IUserService
{
    Task<TaskErrorResult<UserDTO>> GetUserByIdAsync(Guid id);
    Task<TaskErrorResult<IEnumerable<UserDTO>>> GetAllUsersAsync();
    Task<TaskErrorResult<UserDTO>> CreateUserAsync(CreateUserDtoAdmin createUserDto);
    Task<TaskErrorResult<UserDTO>> UpdateUserAsync(Guid id, UpdateUserDto userDto);
    Task<TaskErrorResult<UserDTO>> DeleteUserAsync(Guid id);
    Task<TaskErrorResult<UserDTO>> GetUserByEmailAsync(string email);
    Task<TaskErrorResult<UserDTO>> GetUserByUsernameAsync(string username);
}