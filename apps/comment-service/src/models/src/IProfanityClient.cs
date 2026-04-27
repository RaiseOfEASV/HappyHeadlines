

namespace models;

public interface IProfanityClient
{
    Task<string> FilterAsync(string text);
}