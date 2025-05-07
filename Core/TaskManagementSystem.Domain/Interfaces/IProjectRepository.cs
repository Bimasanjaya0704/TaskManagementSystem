using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Interfaces;

public interface IProjectRepository
{
    Task<ProjectEntity> GetProjectByIdAsync(Guid id);
    Task<IEnumerable<ProjectEntity>> GetAllProjectsAsync();
    Task<ProjectEntity> AddProjectAsync(ProjectEntity project);
    Task<ProjectEntity> UpdateProjectAsync(Guid id, ProjectEntity project);
    Task<bool> DeleteProjectAsync(Guid id);
    Task<IEnumerable<ProjectEntity>> GetByUserIdAsync(Guid userId);
    Task<bool> ExistsAsync(Guid projectId);
    Task<bool> IsUserMemberAsync(Guid projectId, Guid userId);
}