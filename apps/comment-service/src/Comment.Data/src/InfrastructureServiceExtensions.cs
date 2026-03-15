using Comment.Data.configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options;

namespace Comment.Data;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"];
        });

        services.AddDbContext<CommentDbContext>((sp, options) =>
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            options.UseNpgsql(appOptions.DbConnectionString);
        });

        return services;
    }
}