import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { LearnerStore } from '../../core/state/learner.store';
import { StudySessionState } from '../../domain/models/study-session.model';
import { StudyRepository } from '../../domain/ports/study.repository';

/** Finishes a lesson early, keeping the reward for the cards already answered. */
@Injectable({ providedIn: 'root' })
export class CompleteLessonUseCase {
  private readonly study = inject(StudyRepository);
  private readonly learners = inject(LearnerStore);

  async execute(sessionId: string): Promise<StudySessionState> {
    const state = await firstValueFrom(this.study.complete(sessionId));
    this.learners.set(state.learner);
    return state;
  }
}
