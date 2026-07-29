import { Injectable, computed, inject, signal } from '@angular/core';
import { CompleteLessonUseCase } from '../../../application/study/complete-lesson.use-case';
import { StartLessonUseCase } from '../../../application/study/start-lesson.use-case';
import { GradeCardUseCase } from '../../../application/study/grade-card.use-case';
import {
  StudySession,
  cardNumber,
  currentCard,
  isFinished,
} from '../../../domain/models/study-session.model';

/**
 * State for one lesson.
 *
 * The server owns the queue and the scoring, so this store never advances the lesson itself:
 * it sends a grade and adopts whatever session the API returns. That is what makes closing
 * the mini app mid-lesson safe.
 */
@Injectable()
export class StudyStore {
  private readonly startLesson = inject(StartLessonUseCase);
  private readonly gradeCardUseCase = inject(GradeCardUseCase);
  private readonly completeLesson = inject(CompleteLessonUseCase);

  private readonly sessionState = signal<StudySession | null>(null);
  private readonly loadingState = signal(true);
  private readonly gradingState = signal(false);
  private readonly failedState = signal(false);
  /** Bumped when a grade fails, so the card component can undo its swipe animation. */
  private readonly revisionState = signal(0);

  readonly session = this.sessionState.asReadonly();
  readonly isLoading = this.loadingState.asReadonly();
  readonly isGrading = this.gradingState.asReadonly();
  readonly hasFailed = this.failedState.asReadonly();
  readonly revision = this.revisionState.asReadonly();

  readonly card = computed(() => {
    const session = this.sessionState();
    return session ? currentCard(session) : undefined;
  });

  readonly isComplete = computed(() => {
    const session = this.sessionState();
    return session ? isFinished(session) : false;
  });

  readonly progress = computed(() => this.sessionState()?.progress ?? 0);

  readonly cardNumber = computed(() => {
    const session = this.sessionState();
    return session ? cardNumber(session) : 0;
  });

  readonly totalCards = computed(() => this.sessionState()?.totalCards ?? 0);

  readonly deckTitle = computed(() => this.sessionState()?.deckTitle ?? 'Lesson');

  async start(deckId: string): Promise<void> {
    this.loadingState.set(true);
    this.failedState.set(false);

    try {
      const state = await this.startLesson.execute(deckId);
      this.sessionState.set(state.session);
    } catch {
      this.failedState.set(true);
    } finally {
      this.loadingState.set(false);
    }
  }

  /**
   * Sends the learner's answer for the card on screen.
   * @returns true when the lesson finished as a result of this grade.
   */
  async grade(known: boolean): Promise<boolean> {
    const session = this.sessionState();
    const card = this.card();
    if (!session || !card || this.gradingState()) {
      return false;
    }

    this.gradingState.set(true);
    try {
      const state = await this.gradeCardUseCase.execute(session.id, card.cardId, known);
      this.sessionState.set(state.session);
      return isFinished(state.session);
    } catch {
      // Put the card back where it was; the interceptor already explained why.
      this.revisionState.update((value) => value + 1);
      return false;
    } finally {
      this.gradingState.set(false);
    }
  }

  /** Ends the lesson early, keeping the reward for what was answered. */
  async finishEarly(): Promise<boolean> {
    const session = this.sessionState();
    if (!session || session.answeredCards === 0) {
      return false;
    }

    try {
      const state = await this.completeLesson.execute(session.id);
      this.sessionState.set(state.session);
      return true;
    } catch {
      return false;
    }
  }
}
