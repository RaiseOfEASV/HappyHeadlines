using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using models.continents;
using Options;

namespace Article.Data.configuration;

public class ArticleDbContextFactory
{
    private readonly IOptionsMonitor<AppOptions> _optionsMonitor;

    
    public ArticleDbContextFactory(IOptionsMonitor<AppOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
        Console.WriteLine(optionsMonitor.CurrentValue.Global);
    }

    public ArticleDbContext CreateDbContextForContinent(Continent continent)
    {
        var connectionString = _optionsMonitor.CurrentValue.GetConnectionString(continent);

        return string.IsNullOrEmpty(connectionString)
            ? throw new InvalidOperationException($"No connection string found for: {continent}")
            : CreateContext(continent, connectionString);
    }

    private static ArticleDbContext CreateContext(Continent continent, string connectionString) =>
        continent switch
        {
            Continent.Global      => new GlobalArticleDbContext(BuildOptions<GlobalArticleDbContext>(connectionString)),
            Continent.Europe      => new EuropeArticleDbContext(BuildOptions<EuropeArticleDbContext>(connectionString)),
            Continent.Africa      => new AfricaArticleDbContext(BuildOptions<AfricaArticleDbContext>(connectionString)),
            Continent.Asia        => new AsiaArticleDbContext(BuildOptions<AsiaArticleDbContext>(connectionString)),
            Continent.Australia   => new AustraliaArticleDbContext(BuildOptions<AustraliaArticleDbContext>(connectionString)),
            Continent.SouthAmerica => new SouthAmericaArticleDbContext(BuildOptions<SouthAmericaArticleDbContext>(connectionString)),
            Continent.NorthAmerica => new NorthAmericaArticleDbContext(BuildOptions<NorthAmericaArticleDbContext>(connectionString)),
            Continent.Antarctica  => new AntarcticaArticleDbContext(BuildOptions<AntarcticaArticleDbContext>(connectionString)),
            _ => throw new InvalidOperationException($"No DbContext configured for continent: {continent}")
        };

    private static DbContextOptions<T> BuildOptions<T>(string connectionString) where T : DbContext =>
        new DbContextOptionsBuilder<T>()
            .UseSqlServer(connectionString)
            .EnableSensitiveDataLogging()
            .Options;
}