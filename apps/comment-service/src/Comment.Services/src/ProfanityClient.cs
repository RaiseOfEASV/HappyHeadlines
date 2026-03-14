using System.Net.Http.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace Comment.Services;

public class ProfanityClient : IProfanityClient
{
    private readonly HttpClient _httpClient;
    private readonly ResiliencePipeline _pipeline;

    public ProfanityClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                // Open the circuit after 50% failures in a 30-second window (min 3 calls)
                SamplingDuration = TimeSpan.FromSeconds(30),
                FailureRatio = 0.5,
                MinimumThroughput = 3,
                // Keep circuit open for 30 seconds before trying again (half-open)
                BreakDuration = TimeSpan.FromSeconds(30),
                ShouldHandle = new PredicateBuilder()
                    .Handle<HttpRequestException>()
                    .Handle<TaskCanceledException>()
                    .Handle<TimeoutRejectedException>(),
            })
            .AddTimeout(TimeSpan.FromSeconds(5))
            .Build();
    }

    public async Task<string> FilterAsync(string text)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var response = await _httpClient.PostAsJsonAsync("/profanity/filter", new { Text = text }, ct);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<FilterResponse>(cancellationToken: ct);
            return result?.Filtered ?? text;
        });
    }

    private record FilterResponse(string Filtered);
}
