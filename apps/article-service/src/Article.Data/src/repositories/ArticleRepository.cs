using System.Runtime.InteropServices.JavaScript;
using Article.Data.configuration;
using Article.Data.entities;
using Article.Services.application_interfaces.ports;
using Domain.valueobjects;
using Microsoft.EntityFrameworkCore;
using models.continents;

namespace Article.Data.repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly ArticleDbContextFactory _factory;
    private readonly IContinentContext _continentContext;

    public ArticleRepository(
        ArticleDbContextFactory factory,
        IContinentContext continentContext)
    {
        _factory = factory;
        _continentContext = continentContext;
    }

    private ArticleDbContext CreateContext()
    {
        return _factory.CreateDbContextForContinent(_continentContext.Continent);
    }

    public async Task<IEnumerable<Domain.Article>> GetAllAsync()
    {
        await using var context = CreateContext();
        var entities = await context.Articles
            .Include(a => a.Authors)
            .AsNoTracking()
            .ToListAsync();
        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<Domain.Article>> GetTopLatestArticles(int count)
    {
        await using var context = CreateContext();
        var cutoff = DateTime.UtcNow.AddDays(-count);
        var entities = await context.Articles
            .Include(a => a.Authors)
            .AsNoTracking()
            .OrderByDescending(a => a.Timestamp>=cutoff)
            .ToListAsync();
        return entities.Select(ToDomain);
    }


    public async Task<Domain.Article?> GetByIdAsync(Guid id)
    {
        await using var context = CreateContext();
        var result = await context.Articles
            .Include(a => a.Authors)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.ArticleId == id);
        return result is null ? null : ToDomain(result);
    }

    public async Task<Domain.Article> CreateAsync(Domain.Article article)
    {
        await using var context = CreateContext();
        context.Articles.Add(ToEntity(article));
        await context.SaveChangesAsync();
        return article;
    }

    public async Task<Domain.Article?> UpdateAsync(Domain.Article article)
    {
        await using var context = CreateContext();
        context.Articles.Update(ToEntity(article));
        await context.SaveChangesAsync();
        return article;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await using var context = CreateContext();

        var article = await context.Articles.FindAsync(id);
        if (article == null) return false;

        context.Articles.Remove(article);
        await context.SaveChangesAsync();
        return true;
    }
    
    private static Domain.Article ToDomain(ArticleEntity entity)
    {
        var article = new Domain.Article(
            ArticleName.From(entity.Name),
            ArticleContent.From(entity.Content),
            ArticleTimestamp.From(entity.Timestamp))
        {
            ArticleId = ArticleId.From(entity.ArticleId)
        };

        article.SetAuthors(entity.Authors.Select(a => AuthorId.From(a.AuthorId)));

        return article;
    }

    private static ArticleEntity ToEntity(Domain.Article article) =>
        new()
        {
            ArticleId = article.ArticleId.Value,
            Name      = article.Name.Value,
            Content   = article.Content.Value,
            Timestamp = article.Timestamp.Value,
            Authors   = article.AuthorIds.Select(a => new ArticleAuthor
            {
                Id       = Guid.NewGuid(),
                AuthorId = a.Value
            }).ToList()
        };
}