using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Newsletter.Services.DTOs;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace Newsletter.Services.Clients;

public class ArticleClient : IArticleClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ArticleClient> _logger;
    private readonly ResiliencePipeline _pipeline;

    public ArticleClient(HttpClient httpClient, ILogger<ArticleClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Circuit breaker opens after 50% failures in a 30-second window (min 3 calls)
        _pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                SamplingDuration = TimeSpan.FromSeconds(30),
                FailureRatio = 0.5,
                MinimumThroughput = 3,
                BreakDuration = TimeSpan.FromSeconds(30),
                ShouldHandle = new PredicateBuilder()
                    .Handle<HttpRequestException>()
                    .Handle<TaskCanceledException>()
                    .Handle<TimeoutRejectedException>(),
            })
            .AddTimeout(TimeSpan.FromSeconds(10))
            .Build();
    }

    public async Task<List<ArticleDto>> GetRecentArticlesAsync(int limit = 10)
    {
        try
        {
            return await _pipeline.ExecuteAsync(async ct =>
            {
                _logger.LogInformation("Fetching recent articles (limit: {Limit})", limit);

                var response = await _httpClient.GetAsync($"/api/article/recent?limit={limit}", ct);
                response.EnsureSuccessStatusCode();

                var articles = await response.Content.ReadFromJsonAsync<List<ArticleDto>>(cancellationToken: ct);

                _logger.LogInformation("Fetched {Count} recent articles", articles?.Count ?? 0);
                return articles ?? new List<ArticleDto>();
            });
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Circuit breaker is open - cannot fetch articles");
            return new List<ArticleDto>();
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "Timeout while fetching articles");
            return new List<ArticleDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch recent articles from ArticleService");
            return new List<ArticleDto>();
        }
    }
}
