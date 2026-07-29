using LingoHop.Application.Study.Dtos;
using LingoHop.Domain.Decks;
using LingoHop.Domain.Study;

namespace LingoHop.Application.Study;

/// <summary>
/// Projects a session together with the deck it draws from - the queue lives in the session
/// aggregate while the words live in the deck aggregate, so both are needed to render a lesson.
/// </summary>
internal static class StudySessionMapper
{
    public static StudySessionDto ToDto(this StudySession session, Deck deck)
    {
        var cardsById = deck.Cards.ToDictionary(card => card.Id);

        var cards = session.Queue
            .Where(slot => cardsById.ContainsKey(slot.CardId))
            .Select(slot =>
            {
                var card = cardsById[slot.CardId];
                return new StudyCardDto
                {
                    CardId = card.Id,
                    Position = slot.Position,
                    Term = card.Term.Value,
                    Translation = card.Translation.Value,
                    PartOfSpeech = card.PartOfSpeech.ToString(),
                    Example = card.Example?.Value,
                    Known = slot.Known,
                };
            })
            .ToList();

        return new StudySessionDto
        {
            Id = session.Id,
            DeckId = deck.Id,
            DeckTitle = deck.Title.Value,
            DeckIcon = deck.Icon.Value,
            Status = session.Status.ToString(),
            TotalCards = session.TotalCards,
            AnsweredCards = session.AnsweredCards,
            KnownCards = session.KnownCards,
            UnknownCards = session.UnknownCards,
            Progress = session.Progress,
            CurrentCardId = session.CurrentCard?.CardId,
            ExperienceEarned = session.ExperienceEarned,
            StartedAtUtc = session.StartedAtUtc,
            CompletedAtUtc = session.CompletedAtUtc,
            Cards = cards,
        };
    }
}
