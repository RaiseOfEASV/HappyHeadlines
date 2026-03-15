using Article.Data.cache;
using Article.Data.configuration;
using Article.Data.repositories;
using Article.Services.application_interfaces.ports;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Article.Data
{
    public static class InfrastructureServiceExtension
    {
        public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Redis cache - use IConfiguration injection so IOptionsMonitor
            // resolves the connection string at options-resolution time, not at
            // service-registration time (avoids null-capture closure issues).
            services.AddStackExchangeRedisCache(_ => { });
            services.AddOptions<RedisCacheOptions>()
                .Configure<IConfiguration>((options, config) =>
                {
                    options.Configuration =
                        config["AppInfrastructureOptions:Redis:ConnectionString"]
                        ?? config["Redis:ConnectionString"]
                        ?? config["AppInfrastructureOptions:RedisConnectionString"]
                        ?? throw new InvalidOperationException(
                            "Redis connection string is not configured. " +
                            "Set AppInfrastructureOptions:Redis:ConnectionString or Redis:ConnectionString.");
                });

            services.AddSingleton<ArticleDbContextFactory>();
            services.AddScoped<IContinentContext, ContinentContext>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }
    }
}