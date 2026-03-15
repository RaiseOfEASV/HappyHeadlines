using System.ComponentModel.DataAnnotations;

namespace Newsletter.Data.entities;

public class NewsletterDeliveryEntity
{
    [Key]
    public Guid DeliveryId { get; set; }

    [Required]
    public Guid NewsletterId { get; set; }

    [Required]
    public Guid SubscriberId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty; // 'queued', 'sent', 'failed', 'bounced'

    public DateTime? SentAt { get; set; }

    public DateTime? OpenedAt { get; set; }

    public string? ErrorMessage { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class DeliveryStatus
{
    public const string Queued = "queued";
    public const string Sent = "sent";
    public const string Failed = "failed";
    public const string Bounced = "bounced";
}
