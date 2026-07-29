using LingoHop.Domain.Common;

namespace LingoHop.Domain.Decks;

/// <summary>The back of a card - the meaning revealed after the flip.</summary>
public sealed record Translation
{
    public const int MaxLength = 200;

    private Translation(string value) => Value = value;

    public string Value { get; }

    public static Translation Create(string value)
    {
        var trimmed = (value ?? string.Empty).Trim();
        DomainException.Require(trimmed.Length > 0, "A card must have a translation on the back.");
        DomainException.Require(
            trimmed.Length <= MaxLength,
            $"A translation must not exceed {MaxLength} characters.");
        return new Translation(trimmed);
    }

    public override string ToString() => Value;
}
