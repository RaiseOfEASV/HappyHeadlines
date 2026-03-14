
using Article.Services.application_interfaces.ports;
using Article.Services.application_services;
using Microsoft.Extensions.DependencyInjection;

namespace Article.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        services.AddScoped<IArticleService, ArticleService>();
        services.AddHostedService<ArticleLoaderBackgroundservice>();
        return services;
    }
}