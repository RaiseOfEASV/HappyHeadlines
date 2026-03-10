namespace Domain.valueobjects;

public sealed class ArticleId
{
    public Guid Value { get; }

    public ArticleId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ArticleId cannot be an empty GUID.", nameof(value));

        Value = value;
    }

    public static ArticleId New() => new(Guid.NewGuid());

    public static ArticleId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
    public override bool Equals(object? obj) => obj is ArticleId other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator Guid(ArticleId id) => id.Value;
}

