namespace Domain.valueobjects;

public sealed class ArticleContent
{
    public const int MaxLength = 100_000;

    public string Value { get; }

    public ArticleContent(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Article content cannot be null or whitespace.", nameof(value));

        if (value.Length > MaxLength)
            throw new ArgumentException($"Article content cannot exceed {MaxLength} characters.", nameof(value));

        Value = value;
    }

    public static ArticleContent From(string value) => new(value);

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is ArticleContent other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(ArticleContent content) => content.Value;
}

