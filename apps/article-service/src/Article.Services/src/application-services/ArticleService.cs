using Article.Services.application_interfaces.ports;
using Domain.valueobjects;
using models.articles;
using models.continents;

namespace Article.Services.application_services;

public class ArticleService(IArticleRepository articleRepository,IContinentContext continentContext,ICacheService cacheService) : IArticleService
{
    public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync(Continent continent)
    {
        continentContext.Continent = continent;
        if (continent == Continent.Global)
        {
            var cachedArticles = await cacheService.GetAsync<ArticleDto[]>($"recentArticles:{Continent.Global}");
            if (cachedArticles!=null)
            {
                return cachedArticles;
            }
        }
        var articles = await articleRepository.GetAllAsync();
        return articles.Select(ToDto);
    }

    public async Task<ArticleDto?> GetArticleByIdAsync(Guid id)
    {
        foreach (var continent in Enum.GetValues<Continent>())
        {
            continentContext.Continent = continent;
            var article = await articleRepository.GetByIdAsync(id);
            if (article is not null)
                return ToDto(article);
        }
        return null;
    }

    public async Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto)
    {
        var article = new Domain.Article(
            ArticleName.From(createArticleDto.Name),
            ArticleContent.From(createArticleDto.Content),
            ArticleTimestamp.Now());

        article.SetAuthors(createArticleDto.AuthorIds.Select(AuthorId.From));

        var created = await articleRepository.CreateAsync(article);
        return ToDto(created);
    }

    public async Task<ArticleDto?> UpdateArticleAsync(Guid id, UpdateArticleDto updateArticleDto)
    {
        var existing = await articleRepository.GetByIdAsync(id);
        if (existing is null)
            return null;

        existing.Name = ArticleName.From(updateArticleDto.Name);
        existing.Content = ArticleContent.From(updateArticleDto.Content);
        existing.SetAuthors(updateArticleDto.AuthorIds.Select(AuthorId.From));

        var updated = await articleRepository.UpdateAsync(existing);
        return updated is null ? null : ToDto(updated);
    }

    public Task<bool> DeleteArticleAsync(Guid id)
        => articleRepository.DeleteAsync(id);


    private static ArticleDto ToDto(Domain.Article article) => new()
    {
        ArticleId = article.ArticleId.Value,
        Name      = article.Name.Value,
        Content   = article.Content.Value,
        Timestamp = article.Timestamp.Value,
        AuthorIds = article.AuthorIds.Select(a => a.Value).ToList()
    };
}