using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Application.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}