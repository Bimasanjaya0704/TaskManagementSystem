using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Result;

namespace TaskManagementSystem.Application.Interfaces;

public interface IFriendshipService
{
    Task<TaskErrorResult<FriendshipDto>> GetFriendshipByIdAsync(Guid friendshipId);
    Task<TaskErrorResult<FriendshipDto>> SendFriendRequestAsync(FriendRequestDto friendRequestDto);
    Task<TaskErrorResult<FriendshipDto>> RespondToFriendRequestAsync(RespondToFriendRequestDto respondDto, Guid currentUserId);
    Task<TaskErrorResult<IEnumerable<FriendshipDto>>> GetPendingFriendRequestsAsync(Guid userId);
    Task<bool> AreFriendsAsync(Guid user1Id, Guid user2Id);
    Task<bool> RemoveFriendshipAsync(Guid user1Id, Guid user2Id);
}