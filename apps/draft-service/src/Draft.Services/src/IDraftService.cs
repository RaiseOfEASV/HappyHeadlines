using Draft.Data.entities;

namespace Draft.Services;

public interface IDraftService
{
    Task<IEnumerable<DraftEntity>> GetAllAsync();
    Task<DraftEntity?> GetByIdAsync(Guid id);
    Task<DraftEntity> CreateAsync(string title, string content, Guid authorId);
    Task<DraftEntity?> UpdateAsync(Guid id, string title, string content);
    Task<bool> DeleteAsync(Guid id);
}
