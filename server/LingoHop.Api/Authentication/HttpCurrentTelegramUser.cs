using System.Globalization;
using System.Security.Claims;
using LingoHop.Application.Abstractions.Security;

namespace LingoHop.Api.Authentication;

/// <summary>
/// Adapter that exposes the authenticated caller to the Application layer without leaking
/// <c>HttpContext</c> into it.
/// </summary>
internal sealed class HttpCurrentTelegramUser(IHttpContextAccessor accessor) : ICurrentTelegramUser
{
    public bool IsAuthenticated => TryReadIdentity(out _);

    public TelegramIdentity Identity => TryReadIdentity(out var identity)
        ? identity
        : throw new InvalidOperationException("The current request is not authenticated.");

    private bool TryReadIdentity(out TelegramIdentity identity)
    {
        identity = null!;

        var principal = accessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        var rawId = principal.FindFirstValue(TelegramAuthentication.TelegramIdClaim);
        if (!long.TryParse(rawId, CultureInfo.InvariantCulture, out var telegramId))
        {
            return false;
        }

        identity = new TelegramIdentity(
            telegramId,
            principal.FindFirstValue(TelegramAuthentication.FirstNameClaim) ?? "Learner",
            principal.FindFirstValue(TelegramAuthentication.LastNameClaim),
            principal.FindFirstValue(TelegramAuthentication.UsernameClaim),
            principal.FindFirstValue(TelegramAuthentication.LanguageCodeClaim));

        return true;
    }
}
