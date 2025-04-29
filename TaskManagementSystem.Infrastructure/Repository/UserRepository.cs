using Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;

namespace Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _appDbContext;

    public UserRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<UserEntity>> GetAllUserAsync()
    {
        return await _appDbContext.Users
            .AsNoTracking() 
            .ToListAsync();
    }

    public async Task<UserEntity> GetUserByIdAsync(int id)
    {
        return await _appDbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id) ?? throw new InvalidOperationException();
    }

    public async Task<UserEntity> AddUserAsync(UserEntity user)
    {
        await _appDbContext.Users.AddAsync(user);
        await SaveChangesAsync();
        return user;
    }

    public async Task<UserEntity> UpdateUserAsync(UserEntity user)
    {
        var existingUser = await _appDbContext.Users.FindAsync(user.Id);
        if (existingUser == null) return null;

        _appDbContext.Entry(existingUser).CurrentValues.SetValues(user);
        await SaveChangesAsync();
        return existingUser;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _appDbContext.Tasks.FindAsync(id);
        if (user == null) return false;

        _appDbContext.Tasks.Remove(user);
        await SaveChangesAsync();
        return true;
    }

    private async Task SaveChangesAsync()
    {
        await _appDbContext.SaveChangesAsync();
    }
}