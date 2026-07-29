using LingoHop.Domain.Common;

namespace LingoHop.Domain.Users;

/// <summary>
/// Identity of a Telegram account. This is the only identifier the mini app can prove,
/// so it is the natural key that links a Telegram user to a <see cref="User"/> aggregate.
/// </summary>
public sealed record TelegramUserId
{
    private TelegramUserId(long value) => Value = value;

    public long Value { get; }

    public static TelegramUserId Create(long value)
    {
        DomainException.Require(value > 0, "Telegram user id must be a positive number.");
        return new TelegramUserId(value);
    }

    public override string ToString() => Value.ToString();
}
