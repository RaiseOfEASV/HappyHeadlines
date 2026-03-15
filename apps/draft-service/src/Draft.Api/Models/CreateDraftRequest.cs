using System.ComponentModel.DataAnnotations;

namespace Draft.Api.Models;

public record CreateDraftRequest
{
    [Required(ErrorMessage = "Title is required")]
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    [MaxLength(500, ErrorMessage = "Title cannot exceed 500 characters")]
    public string Title { get; init; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    [MinLength(1, ErrorMessage = "Content cannot be empty")]
    public string Content { get; init; } = string.Empty;

    [Required(ErrorMessage = "AuthorId is required")]
    public Guid AuthorId { get; init; }
}
