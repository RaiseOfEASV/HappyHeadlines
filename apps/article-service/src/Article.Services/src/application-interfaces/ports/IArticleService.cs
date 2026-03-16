using models.articles;
using models.continents;

namespace Article.Services.application_interfaces.ports;

public interface IArticleService
{
    Task<IEnumerable<ArticleDto>> GetAllArticlesAsync(Continent continent);
    Task<ArticleDto?> GetArticleByIdAsync(Guid id,Continent continent);
    Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto, Continent continent = Continent.Global);
    Task<ArticleDto?> UpdateArticleAsync(Guid id, UpdateArticleDto updateArticleDto);
    Task<bool> DeleteArticleAsync(Guid id);
}