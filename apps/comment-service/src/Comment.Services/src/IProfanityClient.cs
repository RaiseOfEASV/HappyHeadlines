namespace Comment.Services;

public interface IProfanityClient
{
    Task<string> FilterAsync(string text);
}
