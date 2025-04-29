namespace Core.Interfaces;

public class ITaskRepository
{
    Task AddTask(Task task);
    Task<IEnumerable<Task>> GetTasks();
    Task<Task> GetTaskById(int id);
    Task UpdateTask(Task task);
    Task DeleteTask(int id);
}