using Microsoft.EntityFrameworkCore;

namespace Article.Data.configuration;

public class GlobalArticleDbContext(DbContextOptions<GlobalArticleDbContext> options) : ArticleDbContext(options);

public class EuropeArticleDbContext(DbContextOptions<EuropeArticleDbContext> options) : ArticleDbContext(options);

public class AfricaArticleDbContext(DbContextOptions<AfricaArticleDbContext> options) : ArticleDbContext(options);

public class AsiaArticleDbContext(DbContextOptions<AsiaArticleDbContext> options) : ArticleDbContext(options);

public class AustraliaArticleDbContext(DbContextOptions<AustraliaArticleDbContext> options) : ArticleDbContext(options);

public class SouthAmericaArticleDbContext(DbContextOptions<SouthAmericaArticleDbContext> options) : ArticleDbContext(options);

public class NorthAmericaArticleDbContext(DbContextOptions<NorthAmericaArticleDbContext> options) : ArticleDbContext(options);

public class AntarcticaArticleDbContext(DbContextOptions<AntarcticaArticleDbContext> options) : ArticleDbContext(options);