using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Publisher.Services.Clients;

public class ArticleServiceClient : IArticleServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ArticleServiceClient> _logger;

    public ArticleServiceClient(HttpClient httpClient, ILogger<ArticleServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ArticleResponse?> CreateArticleAsync(string continent, ArticleCreateRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/articles/{continent}", request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("ArticleService returned {StatusCode} when creating article for continent {Continent}",
                    response.StatusCode, continent);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ArticleResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call ArticleService for continent {Continent}", continent);
            return null;
        }
    }
}
