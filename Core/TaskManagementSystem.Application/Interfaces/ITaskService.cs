using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Result;

namespace TaskManagementSystem.Application.Interfaces;

public interface ITaskService
{
    Task<TaskErrorResult<TaskDTO>> GetTaskByIdAsync(Guid id);
    Task<TaskErrorResult<IEnumerable<TaskDTO>>> GetAllTasksAsync();
    Task<TaskErrorResult<TaskDTO>> CreateTaskAsync(CreateTaskDto taskDto);
    Task<TaskErrorResult<TaskDTO>> UpdateTaskAsync(Guid id, TaskDTO taskDto);
    Task<TaskErrorResult<TaskDTO>> DeleteTaskAsync(Guid id);

    Task<TaskErrorResult<TaskDTO>> GetTasksByProjectIdAsync(Guid projectId);
    Task<TaskErrorResult<IEnumerable<TaskDTO>>> GetTasksAssignedToUserAsync(Guid userId);
    Task<TaskErrorResult<TaskDTO>> GetTasksReviewedToUserAsync(Guid userId);
    Task<bool> TaskExistsAsync(Guid taskId);
    Task AssignTaskToUserAsync(Guid taskId, Guid userId);
    Task ReviewTaskToUserAsync(Guid taskId, Guid userId);
    Task UpdateTaskStatusAsync(Guid taskId, Guid status);
}