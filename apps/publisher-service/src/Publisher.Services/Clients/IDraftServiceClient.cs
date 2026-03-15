using Publisher.Services.DTOs;

namespace Publisher.Services.Clients;

public interface IDraftServiceClient
{
    Task<DraftDto?> GetDraftByIdAsync(Guid draftId);
}
