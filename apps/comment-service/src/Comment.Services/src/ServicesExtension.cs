using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options;

namespace Comment.Services;

public static class ServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICommentService, CommentService>();

        services.AddHttpClient<IProfanityClient, ProfanityClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            client.BaseAddress = new Uri(options.ProfanityServiceUrl);
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        return services;
    }
}
