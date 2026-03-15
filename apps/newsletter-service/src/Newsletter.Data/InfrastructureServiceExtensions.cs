using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options;
using Newsletter.Data.configuration;

namespace Newsletter.Data;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services)
    {
        // Add PostgreSQL
        services.AddDbContext<NewsletterDbContext>((sp, options) =>
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            options.UseNpgsql(appOptions.DbConnectionString);
        });

        // Add Redis
        services.AddStackExchangeRedisCache(options =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var appOptions = serviceProvider.GetRequiredService<IOptions<AppOptions>>().Value;
            options.Configuration = appOptions.RedisConnectionString;
            options.InstanceName = "newsletter:";
        });

        return services;
    }
}
