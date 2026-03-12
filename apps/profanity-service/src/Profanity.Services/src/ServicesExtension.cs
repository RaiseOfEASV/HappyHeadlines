using Microsoft.Extensions.DependencyInjection;

namespace Profanity.Services;

public static class ServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProfanityService, ProfanityService>();
        return services;
    }
}
