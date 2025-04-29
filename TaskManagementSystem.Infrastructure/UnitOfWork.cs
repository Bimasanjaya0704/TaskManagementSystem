using Infrastructure.Presistence;
using TaskManagementSystem.Domain.Interfaces;
using TaskManagementSystem.Domain.Interfaces.Security;

namespace Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _appDbContext;
    public ITaskRepository TaskRepository { get; }
    public IUserRepository UserRepository { get; }
    public IProjectRepository ProjectRepository { get; }
    public IPasswordHasher PasswordHasher { get; }

    public UnitOfWork(AppDbContext appDbContext, ITaskRepository taskRepository, IUserRepository userRepository, IProjectRepository projectRepository, IPasswordHasher passwordHasher)
    {
        _appDbContext = appDbContext;
        TaskRepository = taskRepository;
        UserRepository = userRepository;
        ProjectRepository = projectRepository;
        PasswordHasher = passwordHasher;
    }

    public async Task<int> CommitAsync()
    {
        return await _appDbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _appDbContext.Dispose();
    }
}