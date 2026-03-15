using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options;
using Subscriber.Data.configuration;

namespace Subscriber.Data;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services)
    {
        services.AddDbContext<SubscriberDbContext>((sp, options) =>
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            options.UseNpgsql(appOptions.DbConnectionString);
        });

        return services;
    }
}
