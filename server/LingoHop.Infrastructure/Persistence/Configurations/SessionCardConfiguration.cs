using LingoHop.Domain.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingoHop.Infrastructure.Persistence.Configurations;

internal sealed class SessionCardConfiguration : IEntityTypeConfiguration<SessionCard>
{
    public void Configure(EntityTypeBuilder<SessionCard> builder)
    {
        builder.ToTable("session_cards");

        builder.HasKey(card => card.Id);

        // Ids come from the domain, not the database - see CardConfiguration for why this matters.
        builder.Property(card => card.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(card => card.SessionId).HasColumnName("session_id").IsRequired();

        // Deliberately not a foreign key: CardId points at another aggregate, and a lesson
        // stays readable even if the card is edited or removed while it is running.
        builder.Property(card => card.CardId).HasColumnName("card_id").IsRequired();

        builder.Property(card => card.Position).HasColumnName("position");
        builder.Property(card => card.Known).HasColumnName("known");
        builder.Property(card => card.AnsweredAtUtc).HasColumnName("answered_at_utc");

        builder.HasIndex(card => new { card.SessionId, card.Position })
            .IsUnique()
            .HasDatabaseName("ix_session_cards_session_id_position");

        builder.HasIndex(card => new { card.SessionId, card.CardId })
            .IsUnique()
            .HasDatabaseName("ix_session_cards_session_id_card_id");
    }
}
