using Publisher.Services.DTOs;

namespace Publisher.Services;

public interface IPublisherService
{
    Task<PublicationDto> PublishDraftAsync(CreatePublicationDto dto);
    Task<PublicationDto?> GetPublicationByIdAsync(Guid publicationId);
    Task<IEnumerable<PublicationDto>> GetPublicationsByPublisherAsync(Guid publisherId);
    Task<PublicationDto?> UpdatePublicationStatusAsync(Guid publicationId, UpdatePublicationStatusDto dto);
    Task<PublicationDto?> RetryPublicationAsync(Guid publicationId);
}
