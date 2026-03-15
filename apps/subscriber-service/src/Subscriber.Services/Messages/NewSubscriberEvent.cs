namespace HappyHeadlines.Contracts;

public record NewSubscriberEvent
{
    public Guid SubscriberId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? Name { get; init; }
    public DateTime SubscribedAt { get; init; }
}
