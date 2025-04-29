using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Result;

namespace TaskManagementSystem.Application.Interfaces;

public interface IUserService
{
    Task<TaskErrorResult<UserDTO>> GetByIdAsync(int id, string token);
    Task<TaskErrorResult<IEnumerable<UserDTO>>> GetAllAsync(string token);
    Task<TaskErrorResult<UserDTO>> UpdateAsync(int id, string token, UserDTO userDto);
    Task<TaskErrorResult<UserDTO>> DeleteAsync(int id, string token);
    Task<TaskErrorResult<UserDTO>> GetUserByEmailAsync(string email, string token);

}