import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { LearnerStore } from '../../core/state/learner.store';
import { StudySessionState } from '../../domain/models/study-session.model';
import { StudyRepository } from '../../domain/ports/study.repository';

/**
 * Records "Know" or "Don't know" for the card on screen.
 *
 * The server is the authority on what happens next: grading the last card in the queue
 * completes the lesson and awards XP and the streak, and the response carries the updated
 * learner - which is why the store is refreshed on every grade.
 */
@Injectable({ providedIn: 'root' })
export class GradeCardUseCase {
  private readonly study = inject(StudyRepository);
  private readonly learners = inject(LearnerStore);

  async execute(sessionId: string, cardId: string, known: boolean): Promise<StudySessionState> {
    const state = await firstValueFrom(this.study.grade(sessionId, cardId, known));
    this.learners.set(state.learner);
    return state;
  }
}
