import { PartOfSpeech } from './deck.model';
import { LearnerProfile } from './learner.model';

export type StudySessionStatus = 'InProgress' | 'Completed' | 'Abandoned';

/** A card as presented during a lesson. */
export interface StudyCard {
  readonly cardId: string;
  /** Zero-based position in the queue. */
  readonly position: number;
  readonly term: string;
  readonly translation: string;
  readonly partOfSpeech: PartOfSpeech;
  readonly example?: string;
  /** `null` while unanswered, otherwise the grade given. */
  readonly known: boolean | null;
}

/** A lesson: a fixed queue of cards plus the grades given so far. */
export interface StudySession {
  readonly id: string;
  readonly deckId: string;
  readonly deckTitle: string;
  readonly deckIcon: string;
  readonly status: StudySessionStatus;

  readonly totalCards: number;
  readonly answeredCards: number;
  readonly knownCards: number;
  readonly unknownCards: number;
  /** Answered share of the queue, 0..1. */
  readonly progress: number;
  readonly currentCardId?: string;

  readonly experienceEarned: number;
  readonly startedAtUtc: Date;
  readonly completedAtUtc?: Date;

  readonly cards: readonly StudyCard[];
}

/**
 * What every study endpoint returns: the lesson plus the learner's recalculated stats,
 * so the UI never has to refetch the profile after grading.
 */
export interface StudySessionState {
  readonly session: StudySession;
  readonly learner: LearnerProfile;
}

/** The card the learner should see now. */
export function currentCard(session: StudySession): StudyCard | undefined {
  if (session.currentCardId) {
    return session.cards.find((card) => card.cardId === session.currentCardId);
  }
  return session.cards.find((card) => card.known === null);
}

export function isFinished(session: StudySession): boolean {
  return session.status !== 'InProgress';
}

/** One-based index of the card on screen, for the "3/12" counter. */
export function cardNumber(session: StudySession): number {
  return Math.min(session.answeredCards + 1, session.totalCards);
}

/** Share of the queue answered, as a whole percentage. */
export function progressPercent(session: StudySession): number {
  return Math.round(session.progress * 100);
}
