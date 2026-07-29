namespace LingoHop.Api.Authentication;

/// <summary>Constants for the Telegram Mini App authentication scheme.</summary>
public static class TelegramAuthentication
{
    /// <summary>Authentication scheme name.</summary>
    public const string SchemeName = "TelegramInitData";

    /// <summary>
    /// Preferred transport: <c>Authorization: tma &lt;initData&gt;</c>, the convention Telegram
    /// documents for Mini App back ends.
    /// </summary>
    public const string AuthorizationPrefix = "tma ";

    /// <summary>Alternative header for clients that cannot set <c>Authorization</c>.</summary>
    public const string InitDataHeader = "X-Telegram-Init-Data";

    /// <summary>
    /// Development-only override that selects which fake learner a request belongs to,
    /// so several users can be simulated without a bot.
    /// </summary>
    public const string DevelopmentUserHeader = "X-Dev-Telegram-Id";

    /// <summary>Claim holding the numeric Telegram user id.</summary>
    public const string TelegramIdClaim = "telegram:id";

    public const string FirstNameClaim = "telegram:first_name";

    public const string LastNameClaim = "telegram:last_name";

    public const string UsernameClaim = "telegram:username";

    public const string LanguageCodeClaim = "telegram:language_code";
}
