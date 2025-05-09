using Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enum;
using TaskManagementSystem.Domain.Interfaces;

namespace Infrastructure.Repository;

public class FriendRequestRepository : IFriendRequestRepository
{
    private readonly AppDbContext _context;

    public FriendRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FriendRequestEntity> GetByIdAsync(Guid friendRequestId)
    {
        return await _context.FriendRequest.FindAsync(friendRequestId);
    }

    public async Task<FriendRequestEntity> GetByUserIdsAsync(Guid senderId, Guid receiverId)
    {
        return await _context.FriendRequest
            .FirstOrDefaultAsync(f =>
                (f.SenderId == senderId && f.ReceiverId == receiverId) ||
                (f.SenderId == receiverId && f.ReceiverId == senderId));
    }

    public async Task<IEnumerable<FriendRequestEntity>> GetByUserIdAsync(Guid userId)
    {
        return await _context.FriendRequest
            .Where(f => f.SenderId == userId || f.ReceiverId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<FriendRequestEntity>> GetPendingRequestsForUserAsync(Guid userId)
    {
        return await _context.FriendRequest
            .Where(f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending)
            .ToListAsync();
    }

    public async Task<FriendRequestEntity> AddAsync(FriendRequestEntity friendRequest)
    {
        _context.FriendRequest.Add(friendRequest);
        await _context.SaveChangesAsync();
        return friendRequest;
    }

    public async Task UpdateAsync(FriendRequestEntity friendRequest)
    {
        _context.Entry(friendRequest).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid friendRequestId)
    {
        var friendRequest = await _context.FriendRequest.FindAsync(friendRequestId);
        if (friendRequest != null)
        {
            _context.FriendRequest.Remove(friendRequest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid friendRequestId)
    {
        return await _context.FriendRequest.AnyAsync(f => f.FriendshipId == friendRequestId);
    }


    public async Task<bool> ExistsByUserIdsAsync(Guid senderId, Guid receiverId)
    {
        return await _context.FriendRequest.AnyAsync(f =>
            (f.SenderId == senderId && f.ReceiverId == receiverId) ||
            (f.SenderId == receiverId && f.ReceiverId == senderId));
    }
}
