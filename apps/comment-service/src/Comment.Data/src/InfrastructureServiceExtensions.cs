using Comment.Data.cache;
using Comment.Data.configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options;
using StackExchange.Redis;

namespace Comment.Data;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CommentDbContext>((sp, options) =>
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            options.UseNpgsql(appOptions.DbConnectionString);
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"];
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]!));

        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }
}