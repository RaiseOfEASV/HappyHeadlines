namespace Publisher.Services.Messages;

public record PublishArticleCommand
{
    public Guid PublicationId { get; init; }
    public Guid DraftId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public Guid PublisherId { get; init; }
    public List<Guid> AuthorIds { get; init; } = new();
    public string Continent { get; init; } = string.Empty;
    public DateTime RequestedAt { get; init; }
}
