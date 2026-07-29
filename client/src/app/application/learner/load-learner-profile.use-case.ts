import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { LearnerStore } from '../../core/state/learner.store';
import { LearnerProfile } from '../../domain/models/learner.model';
import { LearnerRepository } from '../../domain/ports/learner.repository';

/**
 * Loads the current learner and publishes them to the shared store. On the very first call
 * the API also registers the learner, so this doubles as sign-up.
 *
 * Use cases return Promises: RxJS is an infrastructure detail of the HTTP adapters, and the
 * signal stores that consume these are easier to read with `await`.
 */
@Injectable({ providedIn: 'root' })
export class LoadLearnerProfileUseCase {
  private readonly learners = inject(LearnerRepository);
  private readonly store = inject(LearnerStore);

  async execute(): Promise<LearnerProfile> {
    const profile = await firstValueFrom(this.learners.getProfile());
    this.store.set(profile);
    return profile;
  }
}
