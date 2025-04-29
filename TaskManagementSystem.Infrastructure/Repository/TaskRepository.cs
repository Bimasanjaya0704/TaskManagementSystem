using Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;

namespace Infrastructure;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _appDbContext;
    private readonly ILogger<TaskRepository> _logger;

    public TaskRepository(AppDbContext appDbContext, ILogger<TaskRepository> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskEntity>> GetAllTaskAsync()
    {
        _logger.LogInformation("Start, Fetching all tasks from the database.");

        var tasks = await _appDbContext.Tasks
            .AsNoTracking()
            .Include(t => t.Project)  
            .Include(t => t.AssignedToUser)  
            .Include(t => t.ReviewedByUser)
            .ToListAsync();

        _logger.LogInformation("End, Successfully retrieved {Count} tasks.", tasks.Count);
        return tasks;
    }

    public async Task<TaskEntity?> GetTaskByIdAsync(int id)
    {
        _logger.LogInformation("Start, Fetching task with ID: {TaskId}", id);

        var task = await _appDbContext.Tasks
            .AsNoTracking()
            .Include(t => t.Project)  
            .Include(t => t.AssignedToUser)  
            .Include(t => t.ReviewedByUser)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found.", id);
        }
        else
        {
            _logger.LogInformation("End, Successfully retrieved task with ID {TaskId}.", id);
        }

        return task;
    }

    public async Task<TaskEntity> AddTaskAsync(TaskEntity task)
    {
        _logger.LogInformation("Start, Adding a new task to the database: {TaskName}", task.Id);

        await _appDbContext.Tasks.AddAsync(task);
        await SaveChangesAsync();

        _logger.LogInformation("End, Task added successfully with ID {TaskId}.", task.Id);
        return task;
    }

    public async Task<TaskEntity?> UpdateTaskAsync(int id, TaskEntity task)
    {
        _logger.LogInformation("Start, Updating task with ID: {TaskId}", id);

        var existingTask = await _appDbContext.Tasks.FindAsync(id);
        if (existingTask == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found, update failed.", id);
            return null;
        }

        _appDbContext.Entry(existingTask).CurrentValues.SetValues(task);
        await SaveChangesAsync();

        _logger.LogInformation("End, Task with ID {TaskId} updated successfully.", id);
        return existingTask;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        _logger.LogInformation("Start, Delete task with ID: {TaskId}", id);

        var task = await _appDbContext.Tasks.FindAsync(id);
        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found, delete failed.", id);
            return false;
        }

        _appDbContext.Tasks.Remove(task);
        await SaveChangesAsync();

        _logger.LogInformation("End, Delete task with ID: {TaskId}", id);
        return true;
    }

    private async Task SaveChangesAsync()
    {
        try
        {
            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation("Database changes saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving changes to the database.");
            throw;
        }
    }
}