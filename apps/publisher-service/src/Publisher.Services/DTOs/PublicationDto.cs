namespace Publisher.Services.DTOs;

public record PublicationDto(
    Guid Id,
    Guid DraftId,
    Guid? ArticleId,
    Guid PublisherId,
    string Status,
    string Title,
    string Content,
    string? Continent,
    DateTime PublishInitiatedAt,
    DateTime? PublishCompletedAt,
    string? ErrorMessage,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreatePublicationDto(
    Guid DraftId,
    Guid PublisherId,
    string? Continent = null
);

public record UpdatePublicationStatusDto(
    string Status,
    Guid? ArticleId = null,
    string? ErrorMessage = null
);

public record DraftDto(
    Guid Id,
    string Title,
    string Content,
    Guid AuthorId
);
