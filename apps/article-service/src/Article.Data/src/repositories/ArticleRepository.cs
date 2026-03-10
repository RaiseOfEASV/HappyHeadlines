
using Article.Data.configuration;
using Article.Data.entities;
using Article.Services.application_interfaces.ports;
using Domain.valueobjects;
using Microsoft.EntityFrameworkCore;

namespace Article.Data.repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly GlobalArticleDbContext _context;

    public ArticleRepository(GlobalArticleDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Domain.Article>> GetAllAsync()
    {
        var entities = await _context.Articles
            .Include(a => a.Authors)
            .AsNoTracking()
            .ToListAsync();

        return entities.Select(ToDomain);
    }

    public async Task<Domain.Article?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Articles
            .Include(a => a.Authors)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.ArticleId == id);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Domain.Article> CreateAsync(Domain.Article article)
    {
        var entity = ToEntity(article);

        _context.Articles.Add(entity);
        await _context.SaveChangesAsync();
        
        article.ArticleId = ArticleId.From(entity.ArticleId);

        return article;
    }

    public async Task<Domain.Article?> UpdateAsync(Domain.Article article)
    {
        var entity = await _context.Articles
            .Include(a => a.Authors)
            .FirstOrDefaultAsync(a => a.ArticleId == article.ArticleId.Value);

        if (entity is null)
            return null;

        entity.Name = article.Name.Value;
        entity.Content = article.Content.Value;
        entity.Timestamp = article.Timestamp.Value;

        // Sync authors: remove old, add new
        var incomingIds = article.AuthorIds.Select(a => a.Value).ToHashSet();
        var existingIds = entity.Authors.Select(a => a.AuthorId).ToHashSet();

        entity.Authors.RemoveAll(a => !incomingIds.Contains(a.AuthorId));

        foreach (var newId in incomingIds.Except(existingIds))
        {
            entity.Authors.Add(new ArticleAuthor
            {
                Id = Guid.NewGuid(),
                ArticleId = entity.ArticleId,
                AuthorId = newId
            });
        }

        await _context.SaveChangesAsync();

        return ToDomain(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.Articles.FindAsync(id);

        if (entity is null)
            return false;

        _context.Articles.Remove(entity);
        await _context.SaveChangesAsync();

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

