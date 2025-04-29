using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Interfaces;

public interface IUserRepository
{
    Task<UserEntity> GetUserByIdAsync(int id);  
    Task<IEnumerable<UserEntity>> GetAllUserAsync(); 
    Task<UserEntity> AddUserAsync(UserEntity user);  
    Task<UserEntity> UpdateUserAsync(int id, UserEntity user);  
    Task<bool> DeleteUserAsync(int id);
    
    Task<UserEntity> GetUserByEmailAsync(string email);
}