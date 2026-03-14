namespace Draft.Api.Models;

public record CreateDraftRequest(string Title, string Content, Guid AuthorId);
