import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { LearnerStore } from '../../core/state/learner.store';
import { StudySessionState } from '../../domain/models/study-session.model';
import { StudyRepository } from '../../domain/ports/study.repository';

/** Reads a lesson back, used when a study screen is opened directly by URL. */
@Injectable({ providedIn: 'root' })
export class GetLessonUseCase {
  private readonly study = inject(StudyRepository);
  private readonly learners = inject(LearnerStore);

  async execute(sessionId: string): Promise<StudySessionState> {
    const state = await firstValueFrom(this.study.getById(sessionId));
    this.learners.set(state.learner);
    return state;
  }
}
