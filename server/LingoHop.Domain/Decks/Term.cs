using LingoHop.Domain.Common;

namespace LingoHop.Domain.Decks;

/// <summary>
/// The front of a card - the word being learned. Terms are unique within a deck,
/// compared case-insensitively so "Bill" and "bill" cannot both be added.
/// </summary>
public sealed record Term
{
    public const int MaxLength = 120;

    private Term(string value) => Value = value;

    public string Value { get; }

    public static Term Create(string value)
    {
        var trimmed = (value ?? string.Empty).Trim();
        DomainException.Require(trimmed.Length > 0, "A card must have a word on the front.");
        DomainException.Require(trimmed.Length <= MaxLength, $"A word must not exceed {MaxLength} characters.");
        return new Term(trimmed);
    }

    public bool Matches(Term other) => string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    public override string ToString() => Value;
}
