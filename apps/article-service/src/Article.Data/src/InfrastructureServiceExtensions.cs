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

            services.AddSingleton<GlobalArticleDbContext>(sp =>
                (GlobalArticleDbContext)sp.GetRequiredService<ArticleDbContextFactory>()
                    .CreateDbContextForContinent(Continent.Global));

            services.AddSingleton<EuropeArticleDbContext>(sp =>
                (EuropeArticleDbContext)sp.GetRequiredService<ArticleDbContextFactory>()
                    .CreateDbContextForContinent(Continent.Europe));

            services.AddSingleton<AfricaArticleDbContext>(sp =>
                (AfricaArticleDbContext)sp.GetRequiredService<ArticleDbContextFactory>()
                    .CreateDbContextForContinent(Continent.Africa));

            services.AddSingleton<AsiaArticleDbContext>(sp =>
                (AsiaArticleDbContext)sp.GetRequiredService<ArticleDbContextFactory>()
                    .CreateDbContextForContinent(Continent.Asia));

            services.AddSingleton<AustraliaArticleDbContext>(sp =>
                (AustraliaArticleDbContext)sp.GetRequiredService<ArticleDbContextFactory>()
                    .CreateDbContextForContinent(Continent.Australia));

            services.AddSingleton<SouthAmericaArticleDbContext>(sp =>
                (SouthAmericaArticleDbContext)sp.GetRequiredService<ArticleDbContextFactory>()
                    .CreateDbContextForContinent(Continent.SouthAmerica));

            services.AddSingleton<NorthAmericaArticleDbContext>(sp =>
                (NorthAmericaArticleDbContext)sp.GetRequiredService<ArticleDbContextFactory>()
                    .CreateDbContextForContinent(Continent.NorthAmerica));

            services.AddSingleton<AntarcticaArticleDbContext>(sp =>
                (AntarcticaArticleDbContext)sp.GetRequiredService<ArticleDbContextFactory>()
                    .CreateDbContextForContinent(Continent.Antarctica));


            services.AddScoped<IArticleRepository, ArticleRepository>();

            return services;
        }
    }
}