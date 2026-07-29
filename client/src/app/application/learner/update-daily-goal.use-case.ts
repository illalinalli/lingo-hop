import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { LearnerStore } from '../../core/state/learner.store';
import { LearnerProfile } from '../../domain/models/learner.model';
import { LearnerRepository } from '../../domain/ports/learner.repository';

/** Changes how many cards a day count as the daily goal. */
@Injectable({ providedIn: 'root' })
export class UpdateDailyGoalUseCase {
  private readonly learners = inject(LearnerRepository);
  private readonly store = inject(LearnerStore);

  async execute(cardsPerDay: number): Promise<LearnerProfile> {
    const profile = await firstValueFrom(this.learners.updateDailyGoal(cardsPerDay));
    this.store.set(profile);
    return profile;
  }
}
