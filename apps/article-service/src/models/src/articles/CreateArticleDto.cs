namespace models.articles;

public class CreateArticleDto
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<Guid> AuthorIds { get; set; } = new();
}

