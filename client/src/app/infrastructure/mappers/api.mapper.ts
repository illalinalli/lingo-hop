import { Card, DeckDetails, Deck } from '../../domain/models/deck.model';
import { LearnerProfile } from '../../domain/models/learner.model';
import {
  StudyCard,
  StudySession,
  StudySessionState,
} from '../../domain/models/study-session.model';
import {
  CardDto,
  DeckDetailsDto,
  DeckDto,
  LearnerProfileDto,
  StudyCardDto,
  StudySessionDto,
  StudySessionStateDto,
} from '../dto/api.dto';

/** Turns wire DTOs into domain models: nullables become optionals, ISO strings become Dates. */

const optional = (value: string | null | undefined): string | undefined =>
  value === null || value === undefined || value === '' ? undefined : value;

const date = (value: string): Date => new Date(value);

const optionalDate = (value: string | null | undefined): Date | undefined =>
  value ? new Date(value) : undefined;

export function toLearnerProfile(dto: LearnerProfileDto): LearnerProfile {
  return {
    id: dto.id,
    telegramId: dto.telegramId,
    displayName: dto.displayName,
    firstName: dto.firstName,
    username: optional(dto.username),
    languageCode: optional(dto.languageCode),
    level: dto.level,
    experience: dto.experience,
    experienceIntoLevel: dto.experienceIntoLevel,
    experiencePerLevel: dto.experiencePerLevel,
    levelProgress: dto.levelProgress,
    streak: dto.streak,
    longestStreak: dto.longestStreak,
    dailyGoalCards: dto.dailyGoalCards,
    cardsReviewedToday: dto.cardsReviewedToday,
    dailyGoalCompleted: dto.dailyGoalCompleted,
    pendingExperience: dto.pendingExperience,
    deckCount: dto.deckCount,
    createdAtUtc: date(dto.createdAtUtc),
  };
}

export function toCard(dto: CardDto): Card {
  return {
    id: dto.id,
    term: dto.term,
    translation: dto.translation,
    partOfSpeech: dto.partOfSpeech,
    example: optional(dto.example),
    timesSeen: dto.timesSeen,
    timesKnown: dto.timesKnown,
    correctStreak: dto.correctStreak,
    isLearned: dto.isLearned,
    accuracy: dto.accuracy,
    lastReviewedAtUtc: optionalDate(dto.lastReviewedAtUtc),
    createdAtUtc: date(dto.createdAtUtc),
  };
}

export function toDeck(dto: DeckDto): Deck {
  return {
    id: dto.id,
    title: dto.title,
    icon: dto.icon,
    cardCount: dto.cardCount,
    learnedCardCount: dto.learnedCardCount,
    knownCardCount: dto.knownCardCount,
    completion: dto.completion,
    createdAtUtc: date(dto.createdAtUtc),
  };
}

export function toDeckDetails(dto: DeckDetailsDto): DeckDetails {
  return { ...toDeck(dto), cards: (dto.cards ?? []).map(toCard) };
}

export function toStudyCard(dto: StudyCardDto): StudyCard {
  return {
    cardId: dto.cardId,
    position: dto.position,
    term: dto.term,
    translation: dto.translation,
    partOfSpeech: dto.partOfSpeech,
    example: optional(dto.example),
    known: dto.known ?? null,
  };
}

export function toStudySession(dto: StudySessionDto): StudySession {
  return {
    id: dto.id,
    deckId: dto.deckId,
    deckTitle: dto.deckTitle,
    deckIcon: dto.deckIcon,
    status: dto.status,
    totalCards: dto.totalCards,
    answeredCards: dto.answeredCards,
    knownCards: dto.knownCards,
    unknownCards: dto.unknownCards,
    progress: dto.progress,
    currentCardId: optional(dto.currentCardId),
    experienceEarned: dto.experienceEarned,
    startedAtUtc: date(dto.startedAtUtc),
    completedAtUtc: optionalDate(dto.completedAtUtc),
    cards: (dto.cards ?? []).map(toStudyCard),
  };
}

export function toStudySessionState(dto: StudySessionStateDto): StudySessionState {
  return {
    session: toStudySession(dto.session),
    learner: toLearnerProfile(dto.learner),
  };
}
