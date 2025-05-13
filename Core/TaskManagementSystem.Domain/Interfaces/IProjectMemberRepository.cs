using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Interfaces;

public interface IProjectMemberRepository
{
    Task<ProjectMemberEntity> GetByIdAsync(Guid projectMemberId);
    Task<ProjectMemberEntity> GetByProjectAndUserIdAsync(Guid projectId, Guid userId);
    Task<IEnumerable<ProjectMemberEntity>> GetByProjectIdAsync(Guid projectId);
    Task<IEnumerable<ProjectMemberEntity>> GetByUserIdAsync(Guid userId);
    Task<ProjectMemberEntity> AddAsync(ProjectMemberEntity projectMember);
    Task UpdateAsync(ProjectMemberEntity projectMember);
    Task DeleteAsync(Guid projectMemberId);
    Task DeleteByProjectAndUserIdAsync(Guid projectId, Guid userId);
    Task<bool> ExistsAsync(Guid projectMemberId);
    Task<bool> ExistsByProjectAndUserIdAsync(Guid projectId, Guid userId);
}