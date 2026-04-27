using Microsoft.Extensions.DependencyInjection;
using models;

namespace Feature.Flags;

public static class ServiceExtension
{
    public static IServiceCollection AddFeatureFlags(this IServiceCollection services)
    {
        services.AddScoped<CreateCommentWithProfanity>();
        services.AddScoped<CreateCommentWithoutProfanity>();
        services.AddScoped<FeatureRouter>();
        return services;
    }
}