using LingoHop.Domain.Users;

namespace LingoHop.Application.Abstractions;

/// <summary>
/// Resolves the <see cref="User"/> aggregate behind the current request, creating it on the
/// first launch of the mini app. Scoped: the result is cached for the lifetime of a request.
/// </summary>
public interface ICurrentLearner
{
    Task<User> GetAsync(CancellationToken cancellationToken = default);
}
