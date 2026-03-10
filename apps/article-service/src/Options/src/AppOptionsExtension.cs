using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Options;

public static class AppOptionsExtension
{
    public static AppOptions AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var appOptions = new AppOptions();
        configuration.GetSection("AppInfrastructureOptions").Bind(appOptions);
        services.Configure<AppOptions>(configuration.GetSection("AppInfrastructureOptions"));
        return appOptions;
    }
}