using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Interfaces;

public interface IFriendRequestRepository
{
    Task<FriendRequestEntity> GetByIdAsync(Guid friendshipId);
    Task<FriendRequestEntity> GetByUserIdsAsync(Guid user1Id, Guid user2Id);
    Task<IEnumerable<FriendRequestEntity>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<FriendRequestEntity>> GetPendingRequestsForUserAsync(Guid userId);
    Task<FriendRequestEntity> AddAsync(FriendRequestEntity friendship);
    Task UpdateAsync(FriendRequestEntity friendship);
    Task DeleteAsync(Guid friendshipId);
    Task<bool> ExistsAsync(Guid friendshipId);
    Task<bool> ExistsByUserIdsAsync(Guid user1Id, Guid user2Id);
}