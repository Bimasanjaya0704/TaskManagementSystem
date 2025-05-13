using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserContextService> _logger;

    public UserContextService(IHttpContextAccessor httpContextAccessor, ILogger<UserContextService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public Guid GetCurrentUserId()
    {
        var context = _httpContextAccessor.HttpContext;

        if (context?.Items["UserId"] is Guid id)
        {
            _logger.LogInformation("Retrieved UserId from context: {UserId}", id);
            return id;
        }

        _logger.LogWarning("UserId not found in HttpContext.Items");
        return Guid.Empty;
    }

    public Role GetCurrentUserRole()
    {
        var context = _httpContextAccessor.HttpContext;

        if (context?.Items["Role"] is Role userRole)
        {
            _logger.LogInformation("Retrieved Role from context: {Role}", userRole);
            return userRole;
        }

        _logger.LogWarning("Role not found or invalid in HttpContext.Items");
        return Role.Unknown;
    }
}