using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Interfaces;

public interface IUserRepository
{
    Task<UserEntity> GetUserByIdAsync(Guid id);  
    Task<IEnumerable<UserEntity>> GetAllUserAsync(); 
    Task<UserEntity> AddUserAsync(UserEntity user);  
    Task<UserEntity> UpdateUserAsync(Guid id, UserEntity user);  
    Task<bool> DeleteUserAsync(Guid id);
    Task<UserEntity> GetUserByEmailAsync(string email);
    Task<UserEntity> GetByUsernameAsync(string username);
    Task<bool> ExistsAsync(Guid userId);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);

    Task<IEnumerable<UserEntity>> GetFriendsAsync(Guid userId);
    Task<IEnumerable<FriendRequestEntity>> GetPendingFriendRequestsAsync(Guid userId);
}