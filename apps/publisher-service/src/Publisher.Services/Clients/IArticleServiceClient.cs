namespace Publisher.Services.Clients;

public interface IArticleServiceClient
{
    Task<ArticleResponse?> CreateArticleAsync(string continent, ArticleCreateRequest request);
}

public record ArticleCreateRequest(string Name, string Content, List<Guid> AuthorIds);
public record ArticleResponse(Guid ArticleId, string Name, string Content, DateTime Timestamp);
