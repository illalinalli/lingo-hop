import { inject } from '@angular/core';
import { APP_CONFIG } from '../../core/config/app-config';

/**
 * Builds absolute API URLs from the runtime-configured base. One place to change if the
 * API ever moves to a different prefix.
 */
export function apiRoutes() {
  const base = inject(APP_CONFIG).apiBaseUrl;

  return {
    me: `${base}/api/users/me`,
    dailyGoal: `${base}/api/users/me/daily-goal`,

    decks: `${base}/api/decks`,
    deck: (deckId: string) => `${base}/api/decks/${deckId}`,
    deckResetProgress: (deckId: string) => `${base}/api/decks/${deckId}/reset-progress`,

    cards: (deckId: string) => `${base}/api/decks/${deckId}/cards`,
    card: (deckId: string, cardId: string) => `${base}/api/decks/${deckId}/cards/${cardId}`,

    studySessions: `${base}/api/study-sessions`,
    studySession: (sessionId: string) => `${base}/api/study-sessions/${sessionId}`,
    studySessionGrades: (sessionId: string) => `${base}/api/study-sessions/${sessionId}/grades`,
    studySessionComplete: (sessionId: string) => `${base}/api/study-sessions/${sessionId}/complete`,
  };
}
