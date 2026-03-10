using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options;
using Profanity.Data.configuration;

namespace Profanity.Data;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services)
    {
        services.AddDbContext<ProfanityDbContext>((sp, options) =>
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            options.UseNpgsql(appOptions.DbConnectionString);
        });

        return services;
    }
}
