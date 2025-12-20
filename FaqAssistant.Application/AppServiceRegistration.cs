using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FaqAssistant.Application;

public static class AppServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services, handlers, validators, etc.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AppServiceRegistration).Assembly));
        services.AddValidatorsFromAssembly(typeof(AppServiceRegistration).Assembly);
        return services;
    }
}