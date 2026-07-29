namespace LingoHop.Application.Abstractions.Security;

/// <summary>Result of checking a raw <c>initData</c> query string.</summary>
/// <param name="IsValid">Whether the HMAC signature and freshness checks passed.</param>
/// <param name="Identity">The verified user; <c>null</c> when invalid.</param>
/// <param name="Error">Reason for rejection, for logging.</param>
public sealed record TelegramInitDataResult(bool IsValid, TelegramIdentity? Identity, string? Error)
{
    public static TelegramInitDataResult Valid(TelegramIdentity identity) => new(true, identity, null);

    public static TelegramInitDataResult Invalid(string error) => new(false, null, error);
}

/// <summary>
/// Verifies the <c>initData</c> string a Telegram Mini App sends on launch, proving the
/// request really comes from Telegram on behalf of the claimed user.
/// </summary>
public interface ITelegramInitDataValidator
{
    TelegramInitDataResult Validate(string? initData);
}
