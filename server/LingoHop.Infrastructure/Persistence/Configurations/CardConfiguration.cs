using LingoHop.Domain.Decks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingoHop.Infrastructure.Persistence.Configurations;

internal sealed class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("cards");

        builder.HasKey(card => card.Id);

        // The domain assigns ids (Guid v7), never the database. Without this, EF's Guid-key
        // convention marks the key store-generated, and a card added to an already tracked
        // deck is taken for an existing row - producing an UPDATE that matches nothing
        // instead of an INSERT.
        builder.Property(card => card.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(card => card.DeckId).HasColumnName("deck_id").IsRequired();

        builder.Property(card => card.Term)
            .HasColumnName("term")
            .HasMaxLength(Term.MaxLength)
            .HasConversion(term => term.Value, value => Term.Create(value))
            .IsRequired();

        builder.Property(card => card.Translation)
            .HasColumnName("translation")
            .HasMaxLength(Translation.MaxLength)
            .HasConversion(translation => translation.Value, value => Translation.Create(value))
            .IsRequired();

        // Stored as text so the table stays readable in psql.
        builder.Property(card => card.PartOfSpeech)
            .HasColumnName("part_of_speech")
            .HasMaxLength(32)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(card => card.Example)
            .HasColumnName("example")
            .HasMaxLength(ExampleSentence.MaxLength)
            .HasConversion(
                example => example!.Value,
                value => ExampleSentence.CreateOrNull(value)!);

        builder.ComplexProperty(card => card.Mastery, mastery =>
        {
            mastery.Property(m => m.TimesSeen).HasColumnName("times_seen");
            mastery.Property(m => m.TimesKnown).HasColumnName("times_known");
            mastery.Property(m => m.CorrectStreak).HasColumnName("correct_streak");
            mastery.Property(m => m.LastReviewedAtUtc).HasColumnName("last_reviewed_at_utc");
        });

        builder.Property(card => card.CreatedAtUtc).HasColumnName("created_at_utc");

        // Backstop for the case-insensitive uniqueness the Deck aggregate enforces.
        builder.HasIndex(card => new { card.DeckId, card.Term })
            .IsUnique()
            .HasDatabaseName("ix_cards_deck_id_term");
    }
}
