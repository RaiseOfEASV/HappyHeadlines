namespace Newsletter.Services.DTOs;

public record NewsletterDto
{
    public Guid NewsletterId { get; init; }
    public string NewsletterType { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public int RecipientCount { get; init; }
    public DateTime SentAt { get; init; }
    public List<Guid> ArticleIds { get; init; } = new();
}

public record SendNewsletterDto
{
    public string Subject { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
}
