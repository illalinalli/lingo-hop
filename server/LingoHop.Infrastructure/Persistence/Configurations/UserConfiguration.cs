using LingoHop.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LingoHop.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(user => user.TelegramId)
            .HasColumnName("telegram_id")
            .HasConversion(id => id.Value, value => TelegramUserId.Create(value))
            .IsRequired();

        builder.HasIndex(user => user.TelegramId)
            .IsUnique()
            .HasDatabaseName("ix_users_telegram_id");

        // Value objects with several fields map onto columns of the same row.
        builder.ComplexProperty(user => user.Profile, profile =>
        {
            profile.Property(p => p.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(TelegramProfile.MaxNameLength)
                .IsRequired();
            profile.Property(p => p.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(TelegramProfile.MaxNameLength);
            profile.Property(p => p.Username)
                .HasColumnName("username")
                .HasMaxLength(TelegramProfile.MaxNameLength);
            profile.Property(p => p.LanguageCode)
                .HasColumnName("language_code")
                .HasMaxLength(16);
        });

        builder.Property(user => user.Experience)
            .HasColumnName("experience")
            .HasConversion(points => points.Value, value => ExperiencePoints.Create(value))
            .IsRequired();

        builder.ComplexProperty(user => user.Streak, streak =>
        {
            streak.Property(s => s.Current).HasColumnName("streak_current");
            streak.Property(s => s.Longest).HasColumnName("streak_longest");
            streak.Property(s => s.LastStudiedOn).HasColumnName("streak_last_studied_on");
        });

        builder.ComplexProperty(user => user.DailyProgress, progress =>
        {
            progress.Property(p => p.Date).HasColumnName("daily_progress_date");
            progress.Property(p => p.CardsReviewed).HasColumnName("daily_progress_cards_reviewed");
            progress.Property(p => p.PendingExperience).HasColumnName("daily_progress_pending_experience");
        });

        builder.Property(user => user.DailyGoalCards).HasColumnName("daily_goal_cards");
        builder.Property(user => user.CreatedAtUtc).HasColumnName("created_at_utc");

        builder.Ignore(user => user.DomainEvents);
    }
}
