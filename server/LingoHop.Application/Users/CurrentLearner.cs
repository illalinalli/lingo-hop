using LingoHop.Application.Abstractions;
using LingoHop.Application.Abstractions.Security;
using LingoHop.Domain.Users;

namespace LingoHop.Application.Users;

/// <summary>
/// Maps the verified Telegram identity of the request onto a <see cref="User"/> aggregate,
/// registering it on the very first launch. Scoped, so one request resolves the user once.
/// </summary>
internal sealed class CurrentLearner(
    ICurrentTelegramUser currentTelegramUser,
    IUserRepository users,
    IUnitOfWork unitOfWork,
    IClock clock) : ICurrentLearner
{
    private User? _cached;

    public async Task<User> GetAsync(CancellationToken cancellationToken = default)
    {
        if (_cached is not null)
        {
            return _cached;
        }

        var identity = currentTelegramUser.Identity;
        var telegramId = TelegramUserId.Create(identity.TelegramUserId);
        var profile = TelegramProfile.Create(
            identity.FirstName,
            identity.LastName,
            identity.Username,
            identity.LanguageCode);

        var user = await users.FindByTelegramIdAsync(telegramId, cancellationToken);
        if (user is null)
        {
            user = User.Register(telegramId, profile, clock.UtcNow);
            users.Add(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            // Telegram display names change; keep our copy current.
            user.RefreshProfile(profile);
        }

        _cached = user;
        return user;
    }
}
