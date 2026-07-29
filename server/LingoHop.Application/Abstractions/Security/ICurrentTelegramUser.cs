namespace LingoHop.Application.Abstractions.Security;

/// <summary>
/// The authenticated caller of the current request. Implemented by the API layer on top of
/// the ASP.NET Core authentication result, so use cases never touch <c>HttpContext</c>.
/// </summary>
public interface ICurrentTelegramUser
{
    bool IsAuthenticated { get; }

    /// <summary>The verified Telegram identity.</summary>
    /// <exception cref="InvalidOperationException">The request is not authenticated.</exception>
    TelegramIdentity Identity { get; }
}
