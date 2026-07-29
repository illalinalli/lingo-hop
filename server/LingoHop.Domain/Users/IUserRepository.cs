namespace LingoHop.Domain.Users;

/// <summary>
/// Persistence contract for the <see cref="User"/> aggregate.
/// Declared in the Domain layer, implemented by Infrastructure (dependency inversion).
/// </summary>
public interface IUserRepository
{
    Task<User?> FindByTelegramIdAsync(TelegramUserId telegramId, CancellationToken cancellationToken = default);

    Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(User user);
}
