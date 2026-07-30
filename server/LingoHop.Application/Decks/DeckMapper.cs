using LingoHop.Application.Decks.Dtos;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Decks;

/// <summary>Hand-written projections from the Deck aggregate to its transport shapes.</summary>
internal static class DeckMapper
{
    public static DeckDto ToDto(this Deck deck) => new()
    {
        Id = deck.Id,
        Title = deck.Title.Value,
        Icon = deck.Icon.Value,
        CardCount = deck.CardCount,
        LearnedCardCount = deck.LearnedCardCount,
        KnownCardCount = deck.KnownCardCount,
        Completion = deck.Completion,
        CreatedAtUtc = deck.CreatedAtUtc,
    };

    public static DeckDetailsDto ToDetailsDto(this Deck deck) => new()
    {
        Id = deck.Id,
        Title = deck.Title.Value,
        Icon = deck.Icon.Value,
        CardCount = deck.CardCount,
        LearnedCardCount = deck.LearnedCardCount,
        KnownCardCount = deck.KnownCardCount,
        Completion = deck.Completion,
        CreatedAtUtc = deck.CreatedAtUtc,
        Cards = [.. deck.Cards.OrderBy(card => card.CreatedAtUtc).Select(ToDto)],
    };

    public static CardDto ToDto(this Card card) => new()
    {
        Id = card.Id,
        Term = card.Term.Value,
        Translation = card.Translation.Value,
        PartOfSpeech = card.PartOfSpeech.ToString(),
        Example = card.Example?.Value,
        TimesSeen = card.Mastery.TimesSeen,
        TimesKnown = card.Mastery.TimesKnown,
        CorrectStreak = card.Mastery.CorrectStreak,
        IsLearned = card.Mastery.IsLearned,
        Accuracy = card.Mastery.Accuracy,
        LastReviewedAtUtc = card.Mastery.LastReviewedAtUtc,
        CreatedAtUtc = card.CreatedAtUtc,
    };
}
