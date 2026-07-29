using System.ComponentModel.DataAnnotations;

namespace LingoHop.Infrastructure.Telegram;

/// <summary>
/// Bound from the <c>Telegram</c> configuration section.
/// </summary>
public sealed class TelegramOptions
{
    public const string SectionName = "Telegram";

    /// <summary>
    /// Bot token from @BotFather. Keep it out of source control - use an environment
    /// variable (<c>Telegram__BotToken</c>) or a user-secret.
    /// </summary>
    public string BotToken { get; set; } = string.Empty;

    /// <summary>
    /// How long a launch payload stays acceptable. Telegram recommends rejecting old
    /// <c>initData</c> so a leaked payload cannot be replayed indefinitely.
    /// </summary>
    [Range(typeof(TimeSpan), "00:01:00", "7.00:00:00")]
    public TimeSpan InitDataLifetime { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// When <c>true</c>, requests without a valid <c>initData</c> are accepted and mapped to
    /// <see cref="DevelopmentUserId"/>. Intended for running the Angular app against a local
    /// API without a bot; refused outside the Development environment.
    /// </summary>
    public bool AllowDevelopmentFallback { get; set; }

    /// <summary>Telegram id used by the development fallback.</summary>
    public long DevelopmentUserId { get; set; } = 100_000_001;

    /// <summary>Display name used by the development fallback.</summary>
    public string DevelopmentUserFirstName { get; set; } = "Dev";

    public bool HasBotToken => !string.IsNullOrWhiteSpace(BotToken);
}
