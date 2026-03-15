using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Draft.Data.configuration;
using Draft.Data.entities;

namespace Draft.Services;

public class DraftService(DraftDbContext db, ILogger<DraftService> logger) : IDraftService
{
    public async Task<IEnumerable<DraftEntity>> GetAllAsync()
    {
        logger.LogDebug("Fetching all drafts");
        return await db.Drafts.ToListAsync();
    }

    public async Task<DraftEntity?> GetByIdAsync(Guid id)
    {
        logger.LogDebug("Fetching draft {DraftId}", id);
        return await db.Drafts.FindAsync(id);
    }

    public async Task<DraftEntity> CreateAsync(string title, string content, Guid authorId)
    {
        var draft = new DraftEntity
        {
            Id = Guid.NewGuid(),
            Title = title,
            Content = content,
            AuthorId = authorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Drafts.Add(draft);
        await db.SaveChangesAsync();

        logger.LogInformation("Draft created: {DraftId} by author {AuthorId}", draft.Id, authorId);
        return draft;
    }

    public async Task<DraftEntity?> UpdateAsync(Guid id, string title, string content)
    {
        var draft = await db.Drafts.FindAsync(id);
        if (draft is null)
        {
            logger.LogWarning("Draft not found for update: {DraftId}", id);
            return null;
        }

        draft.Title = title;
        draft.Content = content;
        draft.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        logger.LogInformation("Draft updated: {DraftId}", id);
        return draft;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var draft = await db.Drafts.FindAsync(id);
        if (draft is null)
        {
            logger.LogWarning("Draft not found for deletion: {DraftId}", id);
            return false;
        }

        db.Drafts.Remove(draft);
        await db.SaveChangesAsync();

        logger.LogInformation("Draft deleted: {DraftId}", id);
        return true;
    }
}
