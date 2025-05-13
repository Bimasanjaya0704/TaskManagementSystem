using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Result;

namespace TaskManagementSystem.Application.Interfaces;

public interface IProjectService
{
    Task<TaskErrorResult<ProjectDTO>> GetProjectByIdAsync(Guid id);
    Task<TaskErrorResult<IEnumerable<ProjectDTO>>> GetAllProjectAsync();
    Task<TaskErrorResult<ProjectDTO>> CreateProjectAsync(ProjectDTO createProjectDto);
    Task<TaskErrorResult<ProjectDTO>> UpdateProjectAsync(Guid id, ProjectDTO projectDto);
    Task<TaskErrorResult<ProjectDTO>> DeleteProjectAsync(Guid id);
    Task<bool> ProjectExistsAsync(Guid projectId);
    Task<bool> IsUserProjectMemberAsync(Guid projectId, Guid userId);
    Task<TaskErrorResult<IEnumerable<ProjectMemberDto>>> GetProjectMembersAsync(Guid projectId);
    Task<TaskErrorResult<ProjectMemberDto>> InviteUserToProjectAsync(InviteUserToProjectDto inviteDto, Guid inviterUserId);
    Task RemoveUserFromProjectAsync(Guid projectId, Guid userId, Guid currentUserId);
}