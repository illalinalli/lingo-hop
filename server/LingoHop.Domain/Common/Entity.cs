namespace LingoHop.Domain.Common;

/// <summary>
/// Base class for domain entities: identity-based equality, immutable identifier.
/// </summary>
public abstract class Entity
{
    protected Entity(Guid id)
    {
        DomainException.Require(id != Guid.Empty, "Entity identifier cannot be empty.");
        Id = id;
    }

    /// <summary>Constructor used by EF Core materialisation only.</summary>
    protected Entity()
    {
    }

    public Guid Id { get; private set; }

    public override bool Equals(object? obj) =>
        obj is Entity other && other.GetType() == GetType() && other.Id == Id;

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
