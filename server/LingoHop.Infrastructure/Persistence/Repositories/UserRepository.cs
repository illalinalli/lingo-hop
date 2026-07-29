using LingoHop.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace LingoHop.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(LingoHopDbContext context) : IUserRepository
{
    public Task<User?> FindByTelegramIdAsync(
        TelegramUserId telegramId,
        CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(user => user.TelegramId == telegramId, cancellationToken);

    public Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

    public void Add(User user) => context.Users.Add(user);
}
