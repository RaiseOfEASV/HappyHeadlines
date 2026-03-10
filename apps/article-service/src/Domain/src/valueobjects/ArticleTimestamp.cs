namespace Domain.valueobjects;

public sealed class ArticleTimestamp
{
    public DateTime Value { get; }

    public ArticleTimestamp(DateTime value)
    {
        if (value == default)
            throw new ArgumentException("Article timestamp cannot be the default DateTime value.", nameof(value));

        if (value > DateTime.UtcNow.AddMinutes(1))
            throw new ArgumentException("Article timestamp cannot be in the future.", nameof(value));

        Value = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
    }

    public static ArticleTimestamp Now() => new(DateTime.UtcNow);

    public static ArticleTimestamp From(DateTime value) => new(value);

    public override string ToString() => Value.ToString("o");
    public override bool Equals(object? obj) => obj is ArticleTimestamp other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator DateTime(ArticleTimestamp ts) => ts.Value;
}

