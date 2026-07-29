using LingoHop.Domain.Decks;
using LingoHop.Domain.Study;
using LingoHop.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace LingoHop.Infrastructure.Persistence;

/// <summary>
/// EF Core mapping of the three aggregates. Configuration lives in
/// <c>Persistence/Configurations</c>, one class per aggregate.
/// </summary>
public sealed class LingoHopDbContext(DbContextOptions<LingoHopDbContext> options) : DbContext(options)
{
    /// <summary>Schema all LingoHop tables live in.</summary>
    public const string Schema = "lingohop";

    public DbSet<User> Users => Set<User>();

    public DbSet<Deck> Decks => Set<Deck>();

    public DbSet<StudySession> StudySessions => Set<StudySession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LingoHopDbContext).Assembly);
    }
}
