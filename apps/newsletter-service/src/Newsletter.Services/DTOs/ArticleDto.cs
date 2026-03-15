namespace Newsletter.Services.DTOs;

public record ArticleDto
{
    public Guid ArticleId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Continent { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
