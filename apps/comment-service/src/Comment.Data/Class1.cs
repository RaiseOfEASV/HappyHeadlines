using System.ComponentModel.DataAnnotations;

namespace Comment.Data.entities;

public class CommentEntity
{
    [Key]
    public Guid CommentId { get; set; }

    [Required]
    public Guid ArticleId { get; set; }

    [Required]
    public Guid AuthorId { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}