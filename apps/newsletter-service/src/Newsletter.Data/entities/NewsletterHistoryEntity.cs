using System.ComponentModel.DataAnnotations;

namespace Newsletter.Data.entities;

public class NewsletterHistoryEntity
{
    [Key]
    public Guid NewsletterId { get; set; }

    [Required]
    [MaxLength(50)]
    public string NewsletterType { get; set; } = string.Empty; // 'breaking_news' or 'daily_digest'

    [Required]
    [MaxLength(500)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string ContentHtml { get; set; } = string.Empty;

    [Required]
    public string ArticleIds { get; set; } = "[]"; // JSON array

    [Required]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    [Required]
    public int RecipientCount { get; set; } = 0;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class NewsletterType
{
    public const string BreakingNews = "breaking_news";
    public const string DailyDigest = "daily_digest";
}
