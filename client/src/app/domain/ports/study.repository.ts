import { Observable } from 'rxjs';
import { StudySessionState } from '../models/study-session.model';

/** Port for lessons. */
export abstract class StudyRepository {
  /** Starts a lesson, or resumes the learner's unfinished one for that deck. */
  abstract start(deckId: string, cardLimit?: number): Observable<StudySessionState>;

  abstract getById(sessionId: string): Observable<StudySessionState>;

  /** Records "Know" (`true`) or "Don't know" (`false`) for one card. */
  abstract grade(sessionId: string, cardId: string, known: boolean): Observable<StudySessionState>;

  /** Finishes early, keeping the reward for the cards already answered. */
  abstract complete(sessionId: string): Observable<StudySessionState>;

  abstract abandon(sessionId: string): Observable<void>;
}
