using System.ComponentModel.DataAnnotations;

namespace Publisher.Data.entities;

public class PublicationEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid DraftId { get; set; }

    public Guid? ArticleId { get; set; }

    [Required]
    public Guid PublisherId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty; // 'Draft', 'Publishing', 'Published', 'Failed'

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Continent { get; set; }

    [Required]
    public DateTime PublishInitiatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PublishCompletedAt { get; set; }

    public string? ErrorMessage { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public static class PublicationStatus
{
    public const string Draft = "Draft";
    public const string Publishing = "Publishing";
    public const string Published = "Published";
    public const string Failed = "Failed";
}
