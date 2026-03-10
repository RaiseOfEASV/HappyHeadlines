namespace Profanity.Services;

public interface IProfanityService
{
    Task<string> FilterAsync(string text);
    Task<IEnumerable<string>> GetWordsAsync();
    Task AddWordAsync(string word);
}
