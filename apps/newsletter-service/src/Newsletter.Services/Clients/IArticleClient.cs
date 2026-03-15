using Newsletter.Services.DTOs;

namespace Newsletter.Services.Clients;

public interface IArticleClient
{
    Task<List<ArticleDto>> GetRecentArticlesAsync(int limit = 10);
}
