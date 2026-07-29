using LingoHop.Domain.Common;

namespace LingoHop.Domain.Decks;

/// <summary>Human readable name of a deck, e.g. "Café basics".</summary>
public sealed record DeckTitle
{
    public const int MaxLength = 80;

    private DeckTitle(string value) => Value = value;

    public string Value { get; }

    public static DeckTitle Create(string value)
    {
        var trimmed = (value ?? string.Empty).Trim();
        DomainException.Require(trimmed.Length > 0, "Deck title cannot be empty.");
        DomainException.Require(trimmed.Length <= MaxLength, $"Deck title must not exceed {MaxLength} characters.");
        return new DeckTitle(trimmed);
    }

    public override string ToString() => Value;
}
