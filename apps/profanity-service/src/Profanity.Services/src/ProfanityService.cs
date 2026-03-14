using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Profanity.Data.configuration;
using Profanity.Data.entities;

namespace Profanity.Services;

public class ProfanityService(ProfanityDbContext db, ILogger<ProfanityService> logger) : IProfanityService
{
    public async Task<string> FilterAsync(string text)
    {
        var words = await db.ProfanityWords.Select(w => w.Word).ToListAsync();
        logger.LogDebug("Loaded {WordCount} profanity words for filtering", words.Count);

        var original = text;
        foreach (var word in words)
            text = text.Replace(word, new string('*', word.Length), StringComparison.OrdinalIgnoreCase);

        logger.LogInformation(
            "Text filtered: {WordCount} profanity words checked, content modified: {ContentModified}",
            words.Count,
            original != text);

        return text;
    }

    public async Task<IEnumerable<string>> GetWordsAsync()
    {
        logger.LogDebug("Fetching all profanity words");
        return await db.ProfanityWords.Select(w => w.Word).ToListAsync();
    }

    public async Task AddWordAsync(string word)
    {
        db.ProfanityWords.Add(new ProfanityWordEntity { Id = Guid.NewGuid(), Word = word.ToLower() });
        await db.SaveChangesAsync();
        logger.LogInformation("Profanity word added: {Word}", word.ToLower());
    }
}
