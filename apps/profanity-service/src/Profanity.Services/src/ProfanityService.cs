using Microsoft.EntityFrameworkCore;
using Profanity.Data.configuration;
using Profanity.Data.entities;

namespace Profanity.Services;

public class ProfanityService(ProfanityDbContext db) : IProfanityService
{
    public async Task<string> FilterAsync(string text)
    {
        var words = await db.ProfanityWords.Select(w => w.Word).ToListAsync();
        foreach (var word in words)
            text = text.Replace(word, new string('*', word.Length), StringComparison.OrdinalIgnoreCase);
        return text;
    }

    public async Task<IEnumerable<string>> GetWordsAsync() =>
        await db.ProfanityWords.Select(w => w.Word).ToListAsync();

    public async Task AddWordAsync(string word)
    {
        db.ProfanityWords.Add(new ProfanityWordEntity { Id = Guid.NewGuid(), Word = word.ToLower() });
        await db.SaveChangesAsync();
    }
}
