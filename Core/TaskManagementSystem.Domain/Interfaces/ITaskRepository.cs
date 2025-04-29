using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Interfaces;

public interface ITaskRepository
{
    Task<TaskEntity> GetTaskByIdAsync(int id);  
    Task<IEnumerable<TaskEntity>> GetAllTaskAsync(); 
    Task<TaskEntity> AddTaskAsync(TaskEntity task); 
    Task<TaskEntity> UpdateTaskAsync(int id, TaskEntity task);
    Task<bool> DeleteTaskAsync(int id);
}
