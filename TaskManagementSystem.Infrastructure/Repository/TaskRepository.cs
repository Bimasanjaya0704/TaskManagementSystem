using Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;

namespace Infrastructure;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _appDbContext;

    public TaskRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<TaskEntity>> GetAllTaskAsync()
    {
        return await _appDbContext.Tasks
            .AsNoTracking() 
            .ToListAsync();
    }

    public async Task<TaskEntity> GetTaskByIdAsync(int id)
    {
        return await _appDbContext.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id) ?? throw new InvalidOperationException();
    }

    public async Task<TaskEntity> AddTaskAsync(TaskEntity task)
    {
        await _appDbContext.Tasks.AddAsync(task);
        await SaveChangesAsync();
        return task;
    }

    public async Task<TaskEntity> UpdateTaskAsync(TaskEntity task)
    {
        var existingTask = await _appDbContext.Tasks.FindAsync(task.Id);
        if (existingTask == null) return null;

        _appDbContext.Entry(existingTask).CurrentValues.SetValues(task);
        await SaveChangesAsync();
        return existingTask;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await _appDbContext.Tasks.FindAsync(id);
        if (task == null) return false;

        _appDbContext.Tasks.Remove(task);
        await SaveChangesAsync();
        return true;
    }

    private async Task SaveChangesAsync()
    {
        await _appDbContext.SaveChangesAsync();
    }
}