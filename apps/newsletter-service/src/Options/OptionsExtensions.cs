using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Options;

public static class OptionsExtensions
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppOptions>(configuration.GetSection("AppInfrastructureOptions"));
        return services;
    }
}
