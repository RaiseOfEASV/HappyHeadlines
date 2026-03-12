using System.ComponentModel.DataAnnotations;

namespace Profanity.Data.entities;

public class ProfanityWordEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Word { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
