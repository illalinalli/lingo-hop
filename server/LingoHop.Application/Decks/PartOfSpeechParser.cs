using LingoHop.Application.Common;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Decks;

/// <summary>
/// Translates the free-form grammar tag from the API into the domain enum.
/// Blank input is valid and means <see cref="PartOfSpeech.Unspecified"/>.
/// </summary>
internal static class PartOfSpeechParser
{
    public static Result<PartOfSpeech> Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return PartOfSpeech.Unspecified;
        }

        return Enum.TryParse<PartOfSpeech>(value.Trim(), ignoreCase: true, out var parsed)
            ? parsed
            : Error.Validation(
                "card.invalid_part_of_speech",
                $"\"{value}\" is not a known part of speech. Allowed values: {string.Join(", ", Enum.GetNames<PartOfSpeech>())}.");
    }
}
