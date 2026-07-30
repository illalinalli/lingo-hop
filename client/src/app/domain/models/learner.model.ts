/**
 * The learner behind the current Telegram launch. Mirrors the API's LearnerProfileDto,
 * with dates already parsed.
 */
export interface LearnerProfile {
  readonly id: string;
  readonly telegramId: number;
  readonly displayName: string;
  readonly firstName: string;
  readonly username?: string;
  readonly languageCode?: string;

  readonly level: number;
  readonly experience: number;
  readonly experienceIntoLevel: number;
  readonly experiencePerLevel: number;
  /** Progress through the current level, 0..1. */
  readonly levelProgress: number;

  /** Consecutive study days as of today. */
  readonly streak: number;
  readonly longestStreak: number;

  readonly dailyGoalCards: number;
  readonly cardsReviewedToday: number;
  readonly dailyGoalCompleted: boolean;
  /**
   * XP earned today that today's goal has not released yet. XP is only credited when the
   * daily goal is reached, so this is what the learner still has to unlock.
   */
  readonly pendingExperience: number;

  readonly deckCount: number;
  readonly createdAtUtc: Date;
}

/** Cards still to review before today's goal is met. */
export function cardsLeftToday(learner: LearnerProfile): number {
  return Math.max(0, learner.dailyGoalCards - learner.cardsReviewedToday);
}

/** Share of today's goal already covered, 0..1. */
export function dailyGoalProgress(learner: LearnerProfile): number {
  if (learner.dailyGoalCards <= 0) {
    return 1;
  }
  return Math.min(1, learner.cardsReviewedToday / learner.dailyGoalCards);
}

/** Time-of-day greeting shown above the learner's name. */
export function greetingFor(date: Date = new Date()): string {
  const hour = date.getHours();
  if (hour < 12) {
    return 'Good morning';
  }
  return hour < 18 ? 'Good afternoon' : 'Good evening';
}
