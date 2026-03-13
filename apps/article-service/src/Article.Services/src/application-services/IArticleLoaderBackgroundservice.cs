using Article.Services.application_interfaces.ports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using models.articles;
using models.continents;

namespace Article.Services.application_services;

public class ArticleLoaderBackgroundservice : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(11);
    private static readonly Continent CachedContinent = Continent.Global;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ArticleLoaderBackgroundservice> _logger;

    public ArticleLoaderBackgroundservice(
        IServiceScopeFactory scopeFactory,
        ILogger<ArticleLoaderBackgroundservice> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public static string GetCacheKey(Continent continent) => $"recentArticles:{continent}";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await LoadGlobalAsync(stoppingToken);
        using var timer = new PeriodicTimer(Interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await LoadGlobalAsync(stoppingToken);
        }
    }

    private Task LoadGlobalAsync(CancellationToken ct)
        => LoadContinentAsync(CachedContinent, ct);

    private async Task LoadContinentAsync(Continent continent, CancellationToken ct)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var continentContext = scope.ServiceProvider.GetRequiredService<IContinentContext>();
            var repository = scope.ServiceProvider.GetRequiredService<IArticleRepository>();
            var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
            continentContext.Continent = continent;
            var articles = await repository.GetTopLatestArticles(14);
            var dtos = articles.Select(ToDto).ToList();

            await cache.SetAsync(GetCacheKey(continent), dtos, CacheExpiry);
            _logger.LogInformation("Cached {Count} articles for {Continent}", dtos.Count, continent);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to cache articles for {Continent}", continent);
        }
    }

    private static ArticleDto ToDto(Domain.Article article) => new()
    {
        ArticleId = article.ArticleId.Value,
        Name      = article.Name.Value,
        Content   = article.Content.Value,
        Timestamp = article.Timestamp.Value,
        AuthorIds = article.AuthorIds.Select(a => a.Value).ToList()
    };
}
