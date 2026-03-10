using Domain.valueobjects;

namespace Domain;

public class Article
{
    public ArticleId ArticleId { get; set; } = ArticleId.New();
    public ArticleName Name { get; set; }
    public ArticleContent Content { get; set; }
    public ArticleTimestamp Timestamp { get; set; }
    public List<AuthorId> AuthorIds { get; private set; } = new();

    public Article(ArticleName name, ArticleContent content, ArticleTimestamp timestamp)
    {
        Name = name;
        Content = content;
        Timestamp = timestamp;
    }

    public void AddAuthor(AuthorId authorId)
    {
        if (AuthorIds.Contains(authorId))
            throw new InvalidOperationException($"Author '{authorId}' is already assigned to this article.");

        AuthorIds.Add(authorId);
    }

    public void AddAuthors(IEnumerable<AuthorId> authorIds)
    {
        foreach (var authorId in authorIds)
            AddAuthor(authorId);
    }

    public void SetAuthors(IEnumerable<AuthorId> authorIds)
    {
        var list = authorIds?.ToList() ?? throw new ArgumentNullException(nameof(authorIds));

        if (list.Count == 0)
            throw new ArgumentException("An article must have at least one author.", nameof(authorIds));

        AuthorIds = list;
    }

    public void RemoveAuthor(AuthorId authorId)
    {
        if (!AuthorIds.Remove(authorId))
            throw new InvalidOperationException($"Author '{authorId}' is not assigned to this article.");

        if (AuthorIds.Count == 0)
            throw new InvalidOperationException("An article must have at least one author.");
    }

    public override string ToString() =>
        $"Article {{ Id: {ArticleId}, Name: '{Name}', Timestamp: {Timestamp}, Authors: [{string.Join(", ", AuthorIds)}] }}";
}

