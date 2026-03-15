using Article.Data.cache;
using Article.Data.configuration;
using Article.Data.repositories;
using Article.Services.application_interfaces.ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Article.Data
{
    public static class InfrastructureServiceExtension
    {
        public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["AppInfrastructureOptions:RedisConnectionString"];  
            });

            services.AddSingleton<ArticleDbContextFactory>();
            services.AddScoped<IContinentContext, ContinentContext>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }
    }
}