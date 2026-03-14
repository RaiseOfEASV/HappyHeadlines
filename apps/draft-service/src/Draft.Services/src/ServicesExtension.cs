using Microsoft.Extensions.DependencyInjection;

namespace Draft.Services;

public static class ServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDraftService, DraftService>();
        return services;
    }
}
