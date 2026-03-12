namespace models.articles;

public class ArticleDto
{
    public Guid ArticleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public List<Guid> AuthorIds { get; set; } = new();
}

