using LingoHop.Domain.Common;

namespace LingoHop.Domain.Users;

/// <summary>
/// Snapshot of the display data Telegram hands us in <c>initData</c>.
/// Refreshed on every launch because the user can rename themselves at any time.
/// </summary>
public sealed record TelegramProfile
{
    public const int MaxNameLength = 128;

    private TelegramProfile(string firstName, string? lastName, string? username, string? languageCode)
    {
        FirstName = firstName;
        LastName = lastName;
        Username = username;
        LanguageCode = languageCode;
    }

    public string FirstName { get; }

    public string? LastName { get; }

    public string? Username { get; }

    public string? LanguageCode { get; }

    /// <summary>Name rendered in the greeting header of the mini app.</summary>
    public string DisplayName => string.IsNullOrWhiteSpace(LastName) ? FirstName : $"{FirstName} {LastName}";

    public static TelegramProfile Create(
        string firstName,
        string? lastName = null,
        string? username = null,
        string? languageCode = null)
    {
        var trimmedFirstName = (firstName ?? string.Empty).Trim();
        DomainException.Require(trimmedFirstName.Length > 0, "Telegram profile must have a first name.");
        DomainException.Require(
            trimmedFirstName.Length <= MaxNameLength,
            $"First name must not exceed {MaxNameLength} characters.");

        return new TelegramProfile(
            trimmedFirstName,
            Normalise(lastName, MaxNameLength),
            Normalise(username, MaxNameLength),
            Normalise(languageCode, 16));
    }

    private static string? Normalise(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }
}
