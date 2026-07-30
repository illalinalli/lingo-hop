using Microsoft.Extensions.Configuration;
using Npgsql;

namespace LingoHop.Infrastructure.Persistence;

/// <summary>
/// Resolves the PostgreSQL connection string from configuration, accepting both the .NET form
/// (<c>ConnectionStrings__LingoHopDatabase</c>) and the URL form that managed hosts inject as
/// <c>DATABASE_URL</c>. Npgsql only understands the former, so URLs are translated here.
/// </summary>
internal static class PostgresConnectionString
{
    private const string DatabaseUrlVariable = "DATABASE_URL";

    /// <summary>The connection string in Npgsql form, or <c>null</c> when nothing is configured.</summary>
    public static string? Resolve(IConfiguration configuration)
    {
        // An explicit connection string always wins: it is the one a deployment can tune.
        var configured = configuration.GetConnectionString(DependencyInjection.ConnectionStringName);
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return Normalise(configured);
        }

        var databaseUrl = configuration[DatabaseUrlVariable];

        return string.IsNullOrWhiteSpace(databaseUrl) ? null : Normalise(databaseUrl);
    }

    private static string Normalise(string value) =>
        value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
        || value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
            ? FromUrl(value)
            : value;

    private static string FromUrl(string url)
    {
        var uri = new Uri(url);
        var credentials = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.Trim('/'),
            Username = Uri.UnescapeDataString(credentials[0]),
            Password = credentials.Length > 1 ? Uri.UnescapeDataString(credentials[1]) : string.Empty,
            MaxPoolSize = 20,
        };

        // Providers advertise their TLS requirement in the query string (Neon and Supabase both
        // send sslmode=require). Prefer is the safe default: it encrypts when the server offers
        // TLS, without demanding a CA we cannot verify against.
        builder.SslMode = ReadSslMode(uri.Query);

        return builder.ConnectionString;
    }

    private static SslMode ReadSslMode(string query) =>
        ReadParameter(query, "sslmode") switch
        {
            "disable" => SslMode.Disable,
            "allow" => SslMode.Allow,
            "require" => SslMode.Require,
            "verify-ca" => SslMode.VerifyCA,
            "verify-full" => SslMode.VerifyFull,
            _ => SslMode.Prefer,
        };

    /// <summary>
    /// Reads one query parameter. Deliberately narrow: handing arbitrary URL parameters to
    /// <see cref="NpgsqlConnectionStringBuilder"/> throws on any keyword it does not know.
    /// </summary>
    private static string? ReadParameter(string query, string name)
    {
        foreach (var pair in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=');
            if (separator > 0 && pair.AsSpan(0, separator).Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return Uri.UnescapeDataString(pair[(separator + 1)..]).ToLowerInvariant();
            }
        }

        return null;
    }
}
