using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Interfaces;

public interface IProjectRepository
{
    Task<ProjectEntity> GetProjectByIdAsync(int id);
    Task<IEnumerable<ProjectEntity>> GetAllProjectsAsync();
    Task<ProjectEntity> AddProjectAsync(ProjectEntity project);
    Task<ProjectEntity> UpdateProjectAsync(int id, ProjectEntity project);
    Task<bool> DeleteProjectAsync(int id);
}