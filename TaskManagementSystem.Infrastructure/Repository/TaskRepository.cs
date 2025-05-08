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
            .ToListAsync();

        _logger.LogInformation("End, Successfully retrieved {Count} tasks.", tasks.Count);
        return tasks;
    }

    public async Task<TaskEntity?> GetTaskByIdAsync(Guid id)
    {
        _logger.LogInformation("Start, Fetching task with ID: {TaskId}", id);

        var task = await _appDbContext.Tasks
            .FirstOrDefaultAsync(t => t.TaskId == id);

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
        _logger.LogInformation("Start, Adding a new task to the database: {TaskName}", task.TaskId);

        var project = await _appDbContext.Projects
            .Include(p => p.Tasks) 
            .FirstOrDefaultAsync(p => p.ProjectId == task.ProjectId);

        if (project == null)
        {
            _logger.LogError("Project with ID {ProjectId} not found.", task.ProjectId);
            throw new Exception("Project not found");
        }

        var assignedUser = await _appDbContext.Users.FindAsync(task.AssignedToUserId);
        if (assignedUser == null)
        {
            _logger.LogError("Assigned User with ID {UserId} not found.", task.AssignedToUserId);
            throw new Exception("Assigned User not found");
        }

        var reviewedUser = task.ReviewedToUserId.HasValue ? await _appDbContext.Users.FindAsync(task.ReviewedToUserId.Value) : null;

        project.Tasks.Add(task);
        assignedUser.AssignedTasks.Add(task);

        if (reviewedUser != null)
        {
            reviewedUser.ReviewedTasks.Add(task);
        }

        await _appDbContext.Tasks.AddAsync(task);

        _appDbContext.Projects.Update(project);
        _appDbContext.Users.Update(assignedUser);
        if (reviewedUser != null)
        {
            _appDbContext.Users.Update(reviewedUser);
        }

        await SaveChangesAsync();

        _logger.LogInformation("End, Task added successfully with ID {TaskId}.", task.TaskId);
        return task;
    }

    public async Task<TaskEntity?> UpdateTaskAsync(Guid id, TaskEntity task)
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

    public async Task<bool> DeleteTaskAsync(Guid id)
    {
        _logger.LogInformation("Start, Delete task with ID: {TaskId}", id);

        var task = await _appDbContext.Tasks.FindAsync(id);
        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found, delete failed.", id);
            return false;
        }

        try
        {
            _appDbContext.Tasks.Remove(task);
            await SaveChangesAsync();  

            _logger.LogInformation("End, Delete task with ID: {TaskId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting task with ID {TaskId}: {ErrorMessage}", id, ex.Message);
            return false;
        }
    }
    
    public async Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(Guid projectId)
    {
        return await _appDbContext.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.ReviewedTo)
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskEntity>> GetAssignedToUserAsync(Guid userId)
    {
        return await _appDbContext.Tasks
            .Include(t => t.Project)
            .Where(t => t.AssignedToUserId == userId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<TaskEntity>> GetReviewedToUserAsync(Guid userId)
    {
        return await _appDbContext.Tasks
            .Include(t => t.Project)
            .Where(t => t.ReviewedToUserId == userId)
            .ToListAsync();
    }
    
    public async Task<bool> ExistsAsync(Guid taskId)
    {
        return await _appDbContext.Tasks.AnyAsync(t => t.TaskId == taskId);
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