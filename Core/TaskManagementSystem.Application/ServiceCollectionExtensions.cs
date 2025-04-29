using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Infrastructure;


namespace TaskManagementSystem.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationAndInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);
        return services;
    }
}