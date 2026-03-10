namespace Domain.valueobjects;

public sealed class ArticleName
{
    public const int MaxLength = 200;

    public string Value { get; }

    public ArticleName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Article name cannot be null or whitespace.", nameof(value));

        if (value.Length > MaxLength)
            throw new ArgumentException($"Article name cannot exceed {MaxLength} characters.", nameof(value));

        Value = value.Trim();
    }

    public static ArticleName From(string value) => new(value);

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is ArticleName other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(ArticleName name) => name.Value;
}

