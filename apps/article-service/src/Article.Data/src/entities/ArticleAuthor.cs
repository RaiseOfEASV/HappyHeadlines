using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Article.Data.entities;

public class ArticleAuthor
{
    [Key]
    public Guid Id { get; set; }

    // Guid is a value type — never null, [Required] has no effect
    public Guid ArticleId { get; set; }
    public Guid AuthorId { get; set; }

    // Navigation property back to the owning article
    [ForeignKey(nameof(ArticleId))]
    public ArticleEntity Article { get; set; } = null!;
}