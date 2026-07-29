using LingoHop.Application.Abstractions;
using LingoHop.Application.Abstractions.Events;
using LingoHop.Application.Decks.UseCases;
using LingoHop.Application.Study;
using LingoHop.Application.Study.EventHandlers;
using LingoHop.Application.Study.UseCases;
using LingoHop.Application.Users;
using LingoHop.Application.Users.UseCases;
using LingoHop.Domain.Study.Events;
using Microsoft.Extensions.DependencyInjection;

namespace LingoHop.Application;

/// <summary>
/// Registers the Application layer. Everything is explicit rather than reflection-scanned,
/// so the full list of use cases is readable in one place.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICurrentLearner, CurrentLearner>();
        services.AddScoped<StudySessionStateAssembler>();

        // Users
        services.AddScoped<GetLearnerProfileUseCase>();
        services.AddScoped<UpdateDailyGoalUseCase>();

        // Decks
        services.AddScoped<ListDecksUseCase>();
        services.AddScoped<GetDeckUseCase>();
        services.AddScoped<CreateDeckUseCase>();
        services.AddScoped<UpdateDeckUseCase>();
        services.AddScoped<DeleteDeckUseCase>();
        services.AddScoped<ResetDeckProgressUseCase>();

        // Cards
        services.AddScoped<AddCardUseCase>();
        services.AddScoped<UpdateCardUseCase>();
        services.AddScoped<DeleteCardUseCase>();

        // Study
        services.AddScoped<StartStudySessionUseCase>();
        services.AddScoped<GradeCardUseCase>();
        services.AddScoped<CompleteStudySessionUseCase>();
        services.AddScoped<AbandonStudySessionUseCase>();
        services.AddScoped<GetStudySessionUseCase>();

        // Domain event handlers
        services.AddScoped<IDomainEventHandler<CardReviewedDomainEvent>, UpdateCardMasteryHandler>();
        services.AddScoped<IDomainEventHandler<StudySessionCompletedDomainEvent>, AwardSessionRewardHandler>();

        return services;
    }
}
