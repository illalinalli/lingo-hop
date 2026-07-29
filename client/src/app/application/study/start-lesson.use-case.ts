import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { LearnerStore } from '../../core/state/learner.store';
import { StudySessionState } from '../../domain/models/study-session.model';
import { StudyRepository } from '../../domain/ports/study.repository';

/**
 * Opens a lesson for a deck. The API returns the learner's unfinished lesson for that deck
 * if there is one, so closing the mini app mid-lesson never loses progress.
 */
@Injectable({ providedIn: 'root' })
export class StartLessonUseCase {
  private readonly study = inject(StudyRepository);
  private readonly learners = inject(LearnerStore);

  async execute(deckId: string, cardLimit?: number): Promise<StudySessionState> {
    const state = await firstValueFrom(this.study.start(deckId, cardLimit));
    this.learners.set(state.learner);
    return state;
  }
}
