using LingoHop.Domain.Decks;
using LingoHop.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingoHop.Infrastructure.Persistence.Configurations;

internal sealed class DeckConfiguration : IEntityTypeConfiguration<Deck>
{
    public void Configure(EntityTypeBuilder<Deck> builder)
    {
        builder.ToTable("decks");

        builder.HasKey(deck => deck.Id);
        builder.Property(deck => deck.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(deck => deck.OwnerId).HasColumnName("owner_id").IsRequired();

        builder.Property(deck => deck.Title)
            .HasColumnName("title")
            .HasMaxLength(DeckTitle.MaxLength)
            .HasConversion(title => title.Value, value => DeckTitle.Create(value))
            .IsRequired();

        builder.Property(deck => deck.Icon)
            .HasColumnName("icon")
            .HasMaxLength(DeckIcon.MaxLength)
            .HasConversion(icon => icon.Value, value => DeckIcon.Create(value))
            .IsRequired();

        builder.Property(deck => deck.CreatedAtUtc).HasColumnName("created_at_utc");

        // Deleting a learner removes their decks.
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(deck => deck.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(deck => deck.OwnerId).HasDatabaseName("ix_decks_owner_id");

        // Cards are part of the Deck aggregate: they are always loaded and saved with the root.
        builder.HasMany(deck => deck.Cards)
            .WithOne()
            .HasForeignKey(card => card.DeckId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(deck => deck.Cards)
            .HasField("_cards")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .AutoInclude();

        builder.Ignore(deck => deck.DomainEvents);
    }
}
