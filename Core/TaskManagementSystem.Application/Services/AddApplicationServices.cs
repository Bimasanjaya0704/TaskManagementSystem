using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Mappings;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Application.Services;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddAutoMapper(typeof(DataMapping));

        return services;
    }
}