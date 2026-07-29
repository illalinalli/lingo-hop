using System.Security.Claims;
using System.Text.Encodings.Web;
using LingoHop.Application.Abstractions.Security;
using LingoHop.Infrastructure.Telegram;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace LingoHop.Api.Authentication;

/// <summary>
/// Authenticates a request by verifying the Telegram Mini App <c>initData</c> payload it carries.
/// <para>
/// There are no passwords and no sessions: every call from the mini app re-presents the signed
/// launch payload, and the signature is what proves the caller's identity.
/// </para>
/// </summary>
public sealed class TelegramAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> schemeOptions,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ITelegramInitDataValidator validator,
    IOptions<TelegramOptions> telegramOptions,
    IHostEnvironment environment)
    : AuthenticationHandler<AuthenticationSchemeOptions>(schemeOptions, logger, encoder)
{
    private readonly TelegramOptions _telegram = telegramOptions.Value;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var initData = ReadInitData();

        if (!string.IsNullOrEmpty(initData))
        {
            var result = validator.Validate(initData);
            if (result is { IsValid: true, Identity: not null })
            {
                return Task.FromResult(Success(result.Identity));
            }

            Logger.LogWarning("Rejected a Telegram launch payload: {Reason}", result.Error);
            return Task.FromResult(AuthenticateResult.Fail(result.Error ?? "Invalid initData."));
        }

        if (IsDevelopmentFallbackAllowed())
        {
            return Task.FromResult(Success(BuildDevelopmentIdentity()));
        }

        return Task.FromResult(AuthenticateResult.NoResult());
    }

    private string? ReadInitData()
    {
        var authorization = Request.Headers.Authorization.ToString();
        if (authorization.StartsWith(TelegramAuthentication.AuthorizationPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return authorization[TelegramAuthentication.AuthorizationPrefix.Length..].Trim();
        }

        return Request.Headers[TelegramAuthentication.InitDataHeader].ToString() is { Length: > 0 } header
            ? header
            : null;
    }

    /// <summary>
    /// The fallback is opt-in through configuration <em>and</em> restricted to the Development
    /// environment, so a misconfigured production deployment cannot accept unsigned requests.
    /// </summary>
    private bool IsDevelopmentFallbackAllowed() =>
        _telegram.AllowDevelopmentFallback && environment.IsDevelopment();

    private TelegramIdentity BuildDevelopmentIdentity()
    {
        var telegramId = long.TryParse(
            Request.Headers[TelegramAuthentication.DevelopmentUserHeader].ToString(),
            out var overridden) && overridden > 0
            ? overridden
            : _telegram.DevelopmentUserId;

        return new TelegramIdentity(
            telegramId,
            _telegram.DevelopmentUserFirstName,
            Username: $"dev_{telegramId}",
            LanguageCode: "en");
    }

    private AuthenticateResult Success(TelegramIdentity identity)
    {
        List<Claim> claims =
        [
            new(TelegramAuthentication.TelegramIdClaim, identity.TelegramUserId.ToString()),
            new(ClaimTypes.NameIdentifier, identity.TelegramUserId.ToString()),
            new(TelegramAuthentication.FirstNameClaim, identity.FirstName),
        ];

        AddIfPresent(claims, TelegramAuthentication.LastNameClaim, identity.LastName);
        AddIfPresent(claims, TelegramAuthentication.UsernameClaim, identity.Username);
        AddIfPresent(claims, TelegramAuthentication.LanguageCodeClaim, identity.LanguageCode);

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, TelegramAuthentication.SchemeName, TelegramAuthentication.FirstNameClaim, null));

        return AuthenticateResult.Success(new AuthenticationTicket(principal, TelegramAuthentication.SchemeName));
    }

    private static void AddIfPresent(List<Claim> claims, string type, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            claims.Add(new Claim(type, value));
        }
    }
}
