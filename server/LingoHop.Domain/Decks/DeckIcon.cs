using LingoHop.Domain.Common;

namespace LingoHop.Domain.Decks;

/// <summary>
/// Short emoji badge rendered on the deck tile. Kept as a value object rather than a raw
/// string so the length limit lives in one place.
/// </summary>
public sealed record DeckIcon
{
    /// <summary>Emoji can be several code points long (skin tones, ZWJ sequences).</summary>
    public const int MaxLength = 16;

    private const string FallbackValue = "📘";

    private DeckIcon(string value) => Value = value;

    public string Value { get; }

    public static DeckIcon Default() => new(FallbackValue);

    public static DeckIcon Create(string? value)
    {
        var trimmed = (value ?? string.Empty).Trim();
        if (trimmed.Length == 0)
        {
            return Default();
        }

        DomainException.Require(trimmed.Length <= MaxLength, $"Deck icon must not exceed {MaxLength} characters.");
        return new DeckIcon(trimmed);
    }

    public override string ToString() => Value;
}
