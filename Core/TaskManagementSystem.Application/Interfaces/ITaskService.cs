using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Result;

namespace TaskManagementSystem.Application.Interfaces;

public interface ITaskService
{
    Task<TaskErrorResult<TaskDTO>> GetByIdAsync(int id);
    Task<TaskErrorResult<IEnumerable<TaskDTO>>> GetAllAsync();
    Task<TaskErrorResult<TaskDTO>> AddAsync(TaskDTO taskDto);
    Task<TaskErrorResult<TaskDTO>> UpdateAsync(int id, TaskDTO taskDto);
    Task<TaskErrorResult<TaskDTO>> DeleteAsync(int id);
}