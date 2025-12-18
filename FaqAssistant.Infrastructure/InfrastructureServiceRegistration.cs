using FaqAssistant.Application.Interfaces.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Application.Interfaces.Services;
using FaqAssistant.Infrastructure.Repositories;
using FaqAssistant.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FaqAssistant.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IHashService, HashService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        // Register HttpContextAccessor for CurrentUserService
        services.AddHttpContextAccessor();

        return services;
    }
}
