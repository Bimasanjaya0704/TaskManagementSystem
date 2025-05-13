using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Interfaces;

public interface ITaskRepository
{
    Task<TaskEntity> GetTaskByIdAsync(Guid id);  
    Task<IEnumerable<TaskEntity>> GetAllTaskAsync(); 
    Task<TaskEntity> AddTaskAsync(TaskEntity task); 
    Task<TaskEntity> UpdateTaskAsync(Guid id, TaskEntity task);
    Task<bool> DeleteTaskAsync(Guid id);
    Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(Guid projectId);
    Task<IEnumerable<TaskEntity>> GetAssignedToUserAsync(Guid userId);
    Task<IEnumerable<TaskEntity>> GetReviewedToUserAsync(Guid userId);
    Task<bool> ExistsAsync(Guid taskId);

}
