using System.ComponentModel.DataAnnotations;

namespace Newsletter.Data.entities;

public class SubscriberEntity
{
    [Key]
    public Guid SubscriberId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Name { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public string Preferences { get; set; } = "{}"; // JSON

    [Required]
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UnsubscribedAt { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
