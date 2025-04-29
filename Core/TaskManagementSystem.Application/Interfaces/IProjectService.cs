using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Result;

namespace TaskManagementSystem.Application.Interfaces;

public interface IProjectService
{
    Task<TaskErrorResult<ProjectDTO>> GetProjectByIdAsync(int id);
    Task<TaskErrorResult<IEnumerable<ProjectDTO>>> GetAllProjectAsync();
    Task<TaskErrorResult<ProjectDTO>> AddProjectAsync(ProjectDTO projectDto);
    Task<TaskErrorResult<ProjectDTO>> UpdateProjectAsync(int id, ProjectDTO projectDto);
    Task<TaskErrorResult<ProjectDTO>> DeleteProjectAsync(int id);
}