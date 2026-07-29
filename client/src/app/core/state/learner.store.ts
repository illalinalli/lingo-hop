import { Injectable, computed, signal } from '@angular/core';
import { LearnerProfile, cardsLeftToday, dailyGoalProgress } from '../../domain/models/learner.model';

/**
 * The learner's stats, shared by every feature: the home header reads them, and the study
 * feature refreshes them from the response of each graded card.
 *
 * Kept in `core/` rather than a feature because it outlives any single screen.
 */
@Injectable({ providedIn: 'root' })
export class LearnerStore {
  private readonly state = signal<LearnerProfile | null>(null);

  readonly profile = this.state.asReadonly();

  readonly isLoaded = computed(() => this.state() !== null);

  readonly streak = computed(() => this.state()?.streak ?? 0);

  readonly level = computed(() => this.state()?.level ?? 1);

  readonly experience = computed(() => this.state()?.experience ?? 0);

  readonly levelProgress = computed(() => this.state()?.levelProgress ?? 0);

  readonly dailyGoalCompleted = computed(() => this.state()?.dailyGoalCompleted ?? false);

  readonly cardsLeftToday = computed(() => {
    const profile = this.state();
    return profile ? cardsLeftToday(profile) : 0;
  });

  readonly dailyGoalProgress = computed(() => {
    const profile = this.state();
    return profile ? dailyGoalProgress(profile) : 0;
  });

  set(profile: LearnerProfile): void {
    this.state.set(profile);
  }

  clear(): void {
    this.state.set(null);
  }
}
