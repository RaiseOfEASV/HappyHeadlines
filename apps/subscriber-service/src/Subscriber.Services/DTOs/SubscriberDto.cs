namespace Subscriber.Services.DTOs;

public record SubscriberDto
{
    public Guid SubscriberId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? Name { get; init; }
    public bool IsActive { get; init; }
    public Dictionary<string, object> Preferences { get; init; } = new();
    public DateTime SubscribedAt { get; init; }
    public DateTime? UnsubscribedAt { get; init; }
}

public record CreateSubscriberDto
{
    public string Email { get; init; } = string.Empty;
    public string? Name { get; init; }
    public Dictionary<string, object>? Preferences { get; init; }
}

public record UpdatePreferencesDto
{
    public Dictionary<string, object> Preferences { get; init; } = new();
}
