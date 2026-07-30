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

        // Managed databases terminate TLS with their own CA, so demanding verification would
        // fail on a perfectly good connection. Honour an explicit sslmode when the URL carries one.
        if (!url.Contains("sslmode=", StringComparison.OrdinalIgnoreCase))
        {
            builder.SslMode = SslMode.Prefer;
        }

        return builder.ConnectionString;
    }
}
