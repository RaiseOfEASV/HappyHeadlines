using System.ComponentModel.DataAnnotations;

namespace Article.Data.entities;

public class ArticleEntity
{
    [Key]
    public Guid ArticleId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; }
    
    public List<ArticleAuthor> Authors { get; set; } = new();
}