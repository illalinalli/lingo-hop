import { Observable } from 'rxjs';
import { LearnerProfile } from '../models/learner.model';

/**
 * Port for learner data. Declared as an abstract class so it doubles as an Angular DI
 * token; the HTTP adapter in `infrastructure/` is bound to it in `app.config.ts`.
 */
export abstract class LearnerRepository {
  /** Loads the current learner, registering them on the first launch. */
  abstract getProfile(): Observable<LearnerProfile>;

  abstract updateDailyGoal(cardsPerDay: number): Observable<LearnerProfile>;
}
