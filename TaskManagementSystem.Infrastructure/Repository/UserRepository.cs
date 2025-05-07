using Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enum;
using TaskManagementSystem.Domain.Interfaces;

namespace Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _appDbContext;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AppDbContext appDbContext, ILogger<UserRepository> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<UserEntity>> GetAllUserAsync()
    {
        _logger.LogInformation("Start, Fetching all users from the database.");
        var users = await _appDbContext.Users
            .ToListAsync();
        
        _logger.LogInformation("End, retrieved {Count} users.", users.Count);
        return users;
    }

    public async Task<UserEntity?> GetUserByIdAsync(Guid id)
    {
        _logger.LogInformation("Start, Fetching user with ID: {UserId}", id);

        var user = await _appDbContext.Users
            .FirstOrDefaultAsync(u => u.UserId == id);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", id);
        }
        _logger.LogInformation("End, Fetching user with ID: {UserId}", id);
        return user;
    }

    public async Task<UserEntity> AddUserAsync(UserEntity user)
    {
        _logger.LogInformation("Adding a new user: {UserName}", user.Username);
        await _appDbContext.Users.AddAsync(user);
        await SaveChangesAsync();
        return user;
    }

    public async Task<UserEntity?> UpdateUserAsync(Guid id, UserEntity user)
    {
        _logger.LogInformation("Start, Updating user with ID: {UserId}", id);

        var existingUser = await _appDbContext.Users.FindAsync(id);
        if (existingUser == null)
        {
            _logger.LogWarning("Update failed. User with ID {UserId} not found.", id);
            return null;
        }

        _appDbContext.Entry(existingUser).CurrentValues.SetValues(user);
        await SaveChangesAsync();
        _logger.LogInformation("End, Updating user with ID: {UserId}", id);
        return existingUser;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        _logger.LogInformation("Start, Deleting user with ID: {UserId}", id);

        var user = await _appDbContext.Users.FindAsync(id);
        if (user == null)
        {
            _logger.LogWarning("Delete failed. User with ID {UserId} not found.", id);
            return false;
        }

        _appDbContext.Users.Remove(user);
        await SaveChangesAsync();
        _logger.LogInformation("End, Deleting user with ID: {UserId}", id);
        return true;
    }
    
    public async Task<UserEntity> GetUserByEmailAsync(string email)
    {
        _logger.LogInformation("Start, Fetching user with email: {Email}", email);
        var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found.", email);
        }
        _logger.LogInformation("End, Fetching user with email: {Email}", email);
        return user;
    }
    
    public async Task<UserEntity> GetByUsernameAsync(string username)
    {
        return await _appDbContext.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }
    
    public async Task<bool> ExistsAsync(Guid userId)
    {
        return await _appDbContext.Users.AnyAsync(u => u.UserId == userId);
    }
    
    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _appDbContext.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }
    
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _appDbContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<UserEntity>> GetFriendsAsync(Guid userId)
    {
        // Ambil semua permintaan pertemanan yang telah diterima
        var friendships = await _appDbContext.FriendRequest
            .Where(f => (f.SenderId == userId || f.ReceiverId == userId) &&
                        f.Status == FriendshipStatus.Accepted)
            .ToListAsync();

        // Ambil ID teman (selain userId)
        var friendIds = friendships
            .Select(f => f.SenderId == userId ? f.ReceiverId : f.SenderId)
            .ToList();

        // Ambil data user teman-teman
        var friends = await _appDbContext.Users
            .Where(u => friendIds.Contains(u.UserId))
            .ToListAsync();

        return friends;
    }
    
    public async Task<IEnumerable<FriendRequestEntity>> GetPendingFriendRequestsAsync(Guid userId)
    {
        return await _appDbContext.FriendRequest
            .Include(f => f.Sender)
            .Include(f => f.Receiver)
            .Where(f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending)
            .ToListAsync();
    }

    private async Task SaveChangesAsync()
    {
        try
        {
            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation("Database changes saved successfully.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update error occurred.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while saving changes.");
            throw;
        }
    }
}