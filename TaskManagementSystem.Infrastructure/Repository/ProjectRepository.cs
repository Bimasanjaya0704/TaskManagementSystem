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
    
    public async Task<ProjectEntity?> GetProjectByIdAsync(int id)
    {
        _logger.LogInformation("Start, Fetching project with ID: {ProjectId}", id);

        var project = await _appDbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == id);

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
        _logger.LogInformation("Start, Adding a new project to the database: {ProjectName}", project.Id);

        await _appDbContext.Projects.AddAsync(project);
        await SaveChangesAsync();

        _logger.LogInformation("End, Project added successfully with ID {ProjectId}.", project.Id);
        return project;
    }
    
    public async Task<ProjectEntity?> UpdateProjectAsync(int id, ProjectEntity project)
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
    
    public async Task<bool> DeleteProjectAsync(int id)
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