

namespace Article.Services.application_interfaces.ports;

public interface IArticleRepository
{
    Task<IEnumerable<Domain.Article>> GetAllAsync();
    Task<IEnumerable<Domain.Article>> GetTopLatestArticles(int count);
    
    Task<Domain.Article?> GetByIdAsync(Guid id);
    Task<Domain.Article> CreateAsync(Domain.Article article);
    Task<Domain.Article?> UpdateAsync(Domain.Article article);
    Task<bool> DeleteAsync(Guid id);
}