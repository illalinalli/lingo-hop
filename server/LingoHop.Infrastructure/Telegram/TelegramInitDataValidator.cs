using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LingoHop.Application.Abstractions.Security;
using Microsoft.Extensions.Options;

namespace LingoHop.Infrastructure.Telegram;

/// <summary>
/// Verifies a Telegram Mini App launch payload as described in
/// https://core.telegram.org/bots/webapps#validating-data-received-via-the-mini-app.
/// <para>
/// The check is: build a newline-joined <c>key=value</c> string from every field except
/// <c>hash</c> and <c>signature</c>, sorted by key; HMAC-SHA256 it with a secret derived from
/// the bot token; compare against the supplied <c>hash</c>. Only Telegram and the bot owner
/// know the token, so a matching hash proves the payload was not forged or tampered with.
/// </para>
/// </summary>
internal sealed class TelegramInitDataValidator(IOptions<TelegramOptions> options) : ITelegramInitDataValidator
{
    /// <summary>Fixed key Telegram specifies for deriving the signing secret.</summary>
    private const string SecretKeySalt = "WebAppData";

    /// <summary>Fields that are part of the signature envelope rather than the signed data.</summary>
    private static readonly string[] ExcludedFields = ["hash", "signature"];

    private readonly TelegramOptions _options = options.Value;

    public TelegramInitDataResult Validate(string? initData)
    {
        if (string.IsNullOrWhiteSpace(initData))
        {
            return TelegramInitDataResult.Invalid("initData is empty.");
        }

        if (!_options.HasBotToken)
        {
            return TelegramInitDataResult.Invalid("No Telegram bot token is configured.");
        }

        var fields = ParseFields(initData);

        if (!fields.TryGetValue("hash", out var providedHash) || providedHash.Length == 0)
        {
            return TelegramInitDataResult.Invalid("initData does not contain a hash.");
        }

        if (!IsSignatureValid(fields, providedHash))
        {
            return TelegramInitDataResult.Invalid("initData signature does not match.");
        }

        var freshness = CheckFreshness(fields);
        if (freshness is not null)
        {
            return TelegramInitDataResult.Invalid(freshness);
        }

        if (!fields.TryGetValue("user", out var userJson) || userJson.Length == 0)
        {
            return TelegramInitDataResult.Invalid("initData does not contain a user.");
        }

        var identity = ParseIdentity(userJson);
        return identity is null
            ? TelegramInitDataResult.Invalid("The user field of initData could not be read.")
            : TelegramInitDataResult.Valid(identity);
    }

    private bool IsSignatureValid(IReadOnlyDictionary<string, string> fields, string providedHash)
    {
        var dataCheckString = string.Join(
            '\n',
            fields
                .Where(field => !ExcludedFields.Contains(field.Key))
                .OrderBy(field => field.Key, StringComparer.Ordinal)
                .Select(field => $"{field.Key}={field.Value}"));

        var secretKey = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(SecretKeySalt),
            Encoding.UTF8.GetBytes(_options.BotToken));

        var expectedHash = HMACSHA256.HashData(secretKey, Encoding.UTF8.GetBytes(dataCheckString));

        return TryParseHex(providedHash, out var providedBytes)
               && CryptographicOperations.FixedTimeEquals(expectedHash, providedBytes);
    }

    private string? CheckFreshness(IReadOnlyDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("auth_date", out var rawAuthDate) ||
            !long.TryParse(rawAuthDate, out var unixSeconds))
        {
            return "initData does not contain a valid auth_date.";
        }

        var authDate = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
        var age = DateTimeOffset.UtcNow - authDate;

        return age > _options.InitDataLifetime
            ? $"initData is stale (issued {age.TotalHours:F1} hours ago)."
            : null;
    }

    private static TelegramIdentity? ParseIdentity(string userJson)
    {
        try
        {
            using var document = JsonDocument.Parse(userJson);
            var root = document.RootElement;

            if (!root.TryGetProperty("id", out var idElement) || !idElement.TryGetInt64(out var id))
            {
                return null;
            }

            return new TelegramIdentity(
                id,
                ReadString(root, "first_name") ?? "Learner",
                ReadString(root, "last_name"),
                ReadString(root, "username"),
                ReadString(root, "language_code"));
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? ReadString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    /// <summary>
    /// Decodes the launch payload, which is a form-urlencoded query string.
    /// Values must be compared in their decoded form.
    /// </summary>
    private static Dictionary<string, string> ParseFields(string initData)
    {
        var fields = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var pair in initData.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            var key = Decode(pair[..separator]);
            var value = Decode(pair[(separator + 1)..]);
            fields[key] = value;
        }

        return fields;
    }

    private static string Decode(string value) => Uri.UnescapeDataString(value.Replace('+', ' '));

    private static bool TryParseHex(string hex, out byte[] bytes)
    {
        try
        {
            bytes = Convert.FromHexString(hex);
            return true;
        }
        catch (FormatException)
        {
            bytes = [];
            return false;
        }
    }
}
