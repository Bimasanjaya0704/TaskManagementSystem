using Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;

namespace Infrastructure.Repository;

public class ProjectMemberRepository : IProjectMemberRepository
{
    private readonly AppDbContext _context;

    public ProjectMemberRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectMemberEntity> GetByIdAsync(Guid projectMemberId)
    {
        return await _context.ProjectMembers
            .Include(pm => pm.Project)
            .Include(pm => pm.User)
            .FirstOrDefaultAsync(pm => pm.Id == projectMemberId);
    }

    public async Task<ProjectMemberEntity> GetByProjectAndUserIdAsync(Guid projectId, Guid userId)
    {
        return await _context.ProjectMembers
            .Include(pm => pm.Project)
            .Include(pm => pm.User)
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
    }

    public async Task<IEnumerable<ProjectMemberEntity>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.ProjectMembers
            .Include(pm => pm.User)
            .Where(pm => pm.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectMemberEntity>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ProjectMembers
            .Include(pm => pm.Project)
            .Where(pm => pm.UserId == userId)
            .ToListAsync();
    }

    public async Task<ProjectMemberEntity> AddAsync(ProjectMemberEntity projectMember)
    {
        _context.ProjectMembers.Add(projectMember);
        await _context.SaveChangesAsync();
        return projectMember;
    }

    public async Task UpdateAsync(ProjectMemberEntity projectMember)
    {
        _context.Entry(projectMember).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid projectMemberId)
    {
        var projectMember = await _context.ProjectMembers.FindAsync(projectMemberId);
        if (projectMember != null)
        {
            _context.ProjectMembers.Remove(projectMember);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteByProjectAndUserIdAsync(Guid projectId, Guid userId)
    {
        var projectMember = await _context.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

        if (projectMember != null)
        {
            _context.ProjectMembers.Remove(projectMember);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid projectMemberId)
    {
        return await _context.ProjectMembers.AnyAsync(pm => pm.Id == projectMemberId);
    }

    public async Task<bool> ExistsByProjectAndUserIdAsync(Guid projectId, Guid userId)
    {
        return await _context.ProjectMembers.AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
    }
}
