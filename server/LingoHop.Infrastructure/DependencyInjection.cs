using LingoHop.Application.Abstractions;
using LingoHop.Application.Abstractions.Events;
using LingoHop.Application.Abstractions.Security;
using LingoHop.Domain.Decks;
using LingoHop.Domain.Study;
using LingoHop.Domain.Users;
using LingoHop.Infrastructure.Events;
using LingoHop.Infrastructure.Persistence;
using LingoHop.Infrastructure.Persistence.Repositories;
using LingoHop.Infrastructure.Randomisation;
using LingoHop.Infrastructure.Telegram;
using LingoHop.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LingoHop.Infrastructure;

/// <summary>Wires the adapters behind the Application layer's ports.</summary>
public static class DependencyInjection
{
    /// <summary>Name of the connection string read from configuration.</summary>
    public const string ConnectionStringName = "LingoHopDatabase";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = PostgresConnectionString.Resolve(configuration)
                               ?? throw new InvalidOperationException(
                                   $"Connection string '{ConnectionStringName}' is not configured. " +
                                   "Set ConnectionStrings__LingoHopDatabase or DATABASE_URL in the environment.");

        services.AddDbContext<LingoHopDbContext>(options => options
            .UseNpgsql(connectionString, npgsql => npgsql
                .MigrationsHistoryTable("__ef_migrations_history", LingoHopDbContext.Schema)
                .EnableRetryOnFailure(maxRetryCount: 3)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDeckRepository, DeckRepository>();
        services.AddScoped<IStudySessionRepository, StudySessionRepository>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<ICardShuffler, RandomCardShuffler>();

        services.AddOptions<TelegramOptions>()
            .Bind(configuration.GetSection(TelegramOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ITelegramInitDataValidator, TelegramInitDataValidator>();

        return services;
    }
}
