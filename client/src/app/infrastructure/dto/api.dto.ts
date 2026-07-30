import { PartOfSpeech } from '../../domain/models/deck.model';
import { StudySessionStatus } from '../../domain/models/study-session.model';

/**
 * Wire shapes exactly as the API sends them - dates are ISO strings here and become
 * `Date` objects in the mappers. Keeping DTOs separate from domain models means a change
 * to the API contract is absorbed in one layer.
 */

export interface LearnerProfileDto {
  id: string;
  telegramId: number;
  displayName: string;
  firstName: string;
  username?: string | null;
  languageCode?: string | null;
  level: number;
  experience: number;
  experienceIntoLevel: number;
  experiencePerLevel: number;
  levelProgress: number;
  streak: number;
  longestStreak: number;
  dailyGoalCards: number;
  cardsReviewedToday: number;
  dailyGoalCompleted: boolean;
  pendingExperience: number;
  deckCount: number;
  createdAtUtc: string;
}

export interface CardDto {
  id: string;
  term: string;
  translation: string;
  partOfSpeech: PartOfSpeech;
  example?: string | null;
  timesSeen: number;
  timesKnown: number;
  correctStreak: number;
  isLearned: boolean;
  accuracy: number;
  lastReviewedAtUtc?: string | null;
  createdAtUtc: string;
}

export interface DeckDto {
  id: string;
  title: string;
  icon: string;
  cardCount: number;
  learnedCardCount: number;
  knownCardCount: number;
  completion: number;
  createdAtUtc: string;
}

export interface DeckDetailsDto extends DeckDto {
  cards: CardDto[];
}

export interface StudyCardDto {
  cardId: string;
  position: number;
  term: string;
  translation: string;
  partOfSpeech: PartOfSpeech;
  example?: string | null;
  known?: boolean | null;
}

export interface StudySessionDto {
  id: string;
  deckId: string;
  deckTitle: string;
  deckIcon: string;
  status: StudySessionStatus;
  totalCards: number;
  answeredCards: number;
  knownCards: number;
  unknownCards: number;
  progress: number;
  currentCardId?: string | null;
  experienceEarned: number;
  startedAtUtc: string;
  completedAtUtc?: string | null;
  cards: StudyCardDto[];
}

export interface StudySessionStateDto {
  session: StudySessionDto;
  learner: LearnerProfileDto;
}

/** Request bodies. */

export interface CardDraftDto {
  term: string;
  translation: string;
  partOfSpeech?: string | null;
  example?: string | null;
}

export interface CreateDeckDto {
  title: string;
  icon?: string | null;
  cards?: CardDraftDto[] | null;
}

export interface UpdateDeckDto {
  title: string;
  icon?: string | null;
}

export interface StartStudySessionDto {
  deckId: string;
  cardLimit?: number | null;
}

export interface GradeCardDto {
  cardId: string;
  known: boolean;
}

export interface UpdateDailyGoalDto {
  cardsPerDay: number;
}
