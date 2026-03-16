namespace HappyHeadlines.Contracts;

public record ArticlePublishedEvent
{
    public Guid ArticleId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string Continent { get; init; } = string.Empty;
    public bool IsBreakingNews { get; init; }
}
