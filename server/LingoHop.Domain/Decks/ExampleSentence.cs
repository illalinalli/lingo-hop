using LingoHop.Domain.Common;

namespace LingoHop.Domain.Decks;

/// <summary>Optional usage example shown in italics on the back of a card.</summary>
public sealed record ExampleSentence
{
    public const int MaxLength = 300;

    private ExampleSentence(string value) => Value = value;

    public string Value { get; }

    /// <summary>Returns <c>null</c> for blank input - an example is optional.</summary>
    public static ExampleSentence? CreateOrNull(string? value)
    {
        var trimmed = (value ?? string.Empty).Trim();
        if (trimmed.Length == 0)
        {
            return null;
        }

        DomainException.Require(
            trimmed.Length <= MaxLength,
            $"An example must not exceed {MaxLength} characters.");
        return new ExampleSentence(trimmed);
    }

    public override string ToString() => Value;
}
