using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using Publisher.Services.DTOs;

namespace Publisher.Services.Clients;

public class DraftServiceClient : IDraftServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ResiliencePipeline _pipeline;
    private readonly ILogger<DraftServiceClient> _logger;

    public DraftServiceClient(HttpClient httpClient, ILogger<DraftServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Circuit breaker opens after 50% failures in a 30-second window (min 3 calls)
        // Stays open for 30 seconds before trying again (half-open)
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

    public async Task<DraftDto?> GetDraftByIdAsync(Guid draftId)
    {
        try
        {
            return await _pipeline.ExecuteAsync(async ct =>
            {
                _logger.LogDebug("Fetching draft {DraftId} from DraftService", draftId);

                var response = await _httpClient.GetAsync($"/Draft/{draftId}", ct);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Draft {DraftId} not found in DraftService", draftId);
                    return null;
                }

                response.EnsureSuccessStatusCode();
                var draft = await response.Content.ReadFromJsonAsync<DraftDto>(cancellationToken: ct);

                _logger.LogDebug("Successfully fetched draft {DraftId}", draftId);
                return draft;
            });
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Circuit breaker is open for DraftService");
            throw new InvalidOperationException("DraftService is currently unavailable", ex);
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "Timeout while fetching draft {DraftId}", draftId);
            throw new InvalidOperationException("DraftService request timed out", ex);
        }
    }
}
