namespace LingoHop.Application.Abstractions.Security;

/// <summary>
/// The verified identity carried by a Telegram Mini App launch payload.
/// </summary>
public sealed record TelegramIdentity(
    long TelegramUserId,
    string FirstName,
    string? LastName = null,
    string? Username = null,
    string? LanguageCode = null);
