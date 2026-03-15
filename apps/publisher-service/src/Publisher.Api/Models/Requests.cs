namespace Publisher.Api.Models;

public record PublishRequest(
    Guid DraftId,
    Guid PublisherId,
    string? Continent = null
);

public record UpdateStatusRequest(
    string Status,
    Guid? ArticleId = null,
    string? ErrorMessage = null
);
