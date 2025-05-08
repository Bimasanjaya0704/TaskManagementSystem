using Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;

namespace Infrastructure;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _appDbContext;
    private readonly ILogger<ProjectRepository> _logger;

    public ProjectRepository(AppDbContext appDbContext, ILogger<ProjectRepository> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
    }
    
    public async Task<IEnumerable<ProjectEntity>> GetAllProjectsAsync()
    {
        _logger.LogInformation("Start, Fetching all projects from the database.");

        var projects = await _appDbContext.Projects
            .ToListAsync();

        _logger.LogInformation("End, Successfully retrieved {Count} projects.", projects.Count);
        return projects;
    }
    
    public async Task<ProjectEntity?> GetProjectByIdAsync(Guid id)
    {
        _logger.LogInformation("Start, Fetching project with ID: {ProjectId}", id);

        var project = await _appDbContext.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == id);

        if (project == null)
        {
            _logger.LogWarning("Project with ID {ProjectId} not found.", id);
        }
        else
        {
            _logger.LogInformation("End, Successfully retrieved project with ID {ProjectId}.", id);
        }

        return project;
    }

    public async Task<ProjectEntity> AddProjectAsync(ProjectEntity project)
    {
        _logger.LogInformation("Start, Adding a new project to the database: {ProjectName}", project.ProjectId);

        await _appDbContext.Projects.AddAsync(project);
        await SaveChangesAsync();

        _logger.LogInformation("End, Project added successfully with ID {ProjectId}.", project.ProjectId);
        return project;
    }
    
    public async Task<ProjectEntity?> UpdateProjectAsync(Guid id, ProjectEntity project)
    {
        _logger.LogInformation("Start, Updating project with ID: {ProjectId}", id);

        var existingProject = await _appDbContext.Projects.FindAsync(id);
        if (existingProject == null)
        {
            _logger.LogWarning("Project with ID {ProjectId} not found.", id);
            return null;
        }

        _appDbContext.Entry(existingProject).CurrentValues.SetValues(project);

        await SaveChangesAsync();

        _logger.LogInformation("End, Project updated successfully with ID {ProjectId}.", id);
        return existingProject;
    }
    
    public async Task<bool> DeleteProjectAsync(Guid id)
    {
        _logger.LogInformation("Start, Deleting project with ID: {ProjectId}", id);

        var project = await _appDbContext.Projects.FindAsync(id);
        if (project == null)
        {
            _logger.LogWarning("Project with ID {ProjectId} not found, delete failed.", id);
            return false;
        }

        _appDbContext.Projects.Remove(project);
        await SaveChangesAsync();

        _logger.LogInformation("End, Project with ID {ProjectId} deleted successfully.", id);
        return true;
    }

    public async Task<IEnumerable<ProjectEntity>> GetByUserIdAsync(Guid userId)
    {
        _logger.LogInformation("Start, Fetching projects for user with ID: {UserId}", userId);
        
        var projects = await _appDbContext.ProjectMembers
            .Where(pm => pm.UserId == userId)
            .Include(pm => pm.Project)
            .ThenInclude(p => p.Creator)
            .Select(pm => pm.Project)
            .ToListAsync();

        _logger.LogInformation("End, Successfully retrieved {Count} projects for user with ID: {UserId}.", projects.Count, userId);
        return projects;
    }

    public async Task<bool> ExistsAsync(Guid projectId)
    {
        _logger.LogInformation("Start, Checking existence of project with ID: {ProjectId}", projectId);
        
        var exists = await _appDbContext.Projects.AnyAsync(p => p.ProjectId == projectId);

        if (exists)
        {
            _logger.LogInformation("End, Project with ID {ProjectId} exists.", projectId);
        }
        else
        {
            _logger.LogWarning("End, Project with ID {ProjectId} does not exist.", projectId);
        }

        return exists;
    }

    public async Task<bool> IsUserMemberAsync(Guid projectId, Guid userId)
    {
        _logger.LogInformation("Start, Checking if user with ID: {UserId} is a member of project with ID: {ProjectId}", userId, projectId);

       
        var isMember = await _appDbContext.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

        if (isMember)
        {
            _logger.LogInformation("End, User with ID {UserId} is a member of project with ID: {ProjectId}.", userId, projectId);
        }
        else
        {
            _logger.LogWarning("End, User with ID {UserId} is not a member of project with ID: {ProjectId}.", userId, projectId);
        }

        return isMember;
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