using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.Interfaces;

public interface IUserContextService
{
    Guid GetCurrentUserId();
    Role GetCurrentUserRole();
}