namespace Domain.valueobjects;

public sealed class AuthorId
{
    public Guid Value { get; }

    public AuthorId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("AuthorId cannot be an empty GUID.", nameof(value));

        Value = value;
    }

    public static AuthorId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
    public override bool Equals(object? obj) => obj is AuthorId other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator Guid(AuthorId id) => id.Value;
}

