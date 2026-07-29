using LingoHop.Domain.Decks;
using LingoHop.Domain.Study;
using LingoHop.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingoHop.Infrastructure.Persistence.Configurations;

internal sealed class StudySessionConfiguration : IEntityTypeConfiguration<StudySession>
{
    public void Configure(EntityTypeBuilder<StudySession> builder)
    {
        builder.ToTable("study_sessions");

        builder.HasKey(session => session.Id);
        builder.Property(session => session.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(session => session.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(session => session.DeckId).HasColumnName("deck_id").IsRequired();

        builder.Property(session => session.Status)
            .HasColumnName("status")
            .HasMaxLength(32)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(session => session.StartedAtUtc).HasColumnName("started_at_utc");
        builder.Property(session => session.CompletedAtUtc).HasColumnName("completed_at_utc");
        builder.Property(session => session.ExperienceEarned).HasColumnName("experience_earned");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Deck>()
            .WithMany()
            .HasForeignKey(session => session.DeckId)
            .OnDelete(DeleteBehavior.Cascade);

        // Looking up the learner's unfinished lesson for a deck is the hot path (resume).
        builder.HasIndex(session => new { session.UserId, session.DeckId, session.Status })
            .HasDatabaseName("ix_study_sessions_user_id_deck_id_status");

        builder.HasMany(session => session.Cards)
            .WithOne()
            .HasForeignKey(card => card.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(session => session.Cards)
            .HasField("_cards")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .AutoInclude();

        builder.Ignore(session => session.DomainEvents);
        builder.Ignore(session => session.Queue);
    }
}
