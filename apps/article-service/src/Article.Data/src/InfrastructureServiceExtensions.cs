using Article.Data.configuration;
using Article.Data.repositories;
using Article.Services.application_interfaces.ports;
using Microsoft.Extensions.DependencyInjection;
using models.continents;

namespace Article.Data
{
    public static class InfrastructureServiceExtension
    {
        public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services)
        {
            services.AddSingleton<ArticleDbContextFactory>();
            services.AddScoped<IContinentContext, ContinentContext>();
            services.AddScoped<IArticleRepository, ArticleRepository>();

            return services;
        }
    }
}