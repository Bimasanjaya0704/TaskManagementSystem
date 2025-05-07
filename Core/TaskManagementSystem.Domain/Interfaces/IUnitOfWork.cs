using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces.Security;

namespace TaskManagementSystem.Domain.Interfaces;

public interface IUnitOfWork
{
    ITaskRepository TaskRepository { get; }
    IUserRepository UserRepository { get; }
    IProjectRepository ProjectRepository { get; }
    IFriendRequestRepository FriendRequestRepository { get; }
    IProjectMemberRepository ProjectMemberRepository { get; }
    IPasswordHasher PasswordHasher { get; }
    Task<int> CommitAsync();
}