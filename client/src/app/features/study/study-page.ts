import { ChangeDetectionStrategy, Component, effect, inject, input, viewChild } from '@angular/core';
import { Router } from '@angular/router';
import { LearnerStore } from '../../core/state/learner.store';
import { TelegramService } from '../../core/telegram/telegram.service';
import { EmptyState } from '../../shared/ui/empty-state/empty-state';
import { ProgressBar } from '../../shared/ui/progress-bar/progress-bar';
import { Spinner } from '../../shared/ui/spinner/spinner';
import { StatPill } from '../../shared/ui/stat-pill/stat-pill';
import { ShellStore } from '../shell/state/shell.store';
import { StudyStore } from './state/study.store';
import { Flashcard } from './ui/flashcard/flashcard';
import { LessonSummary } from './ui/lesson-summary/lesson-summary';

/** The lesson screen: progress bar, one card at a time, and the reward sheet at the end. */
@Component({
  selector: 'lh-study-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [EmptyState, Flashcard, LessonSummary, ProgressBar, Spinner, StatPill],
  providers: [StudyStore],
  templateUrl: './study-page.html',
  styleUrl: './study-page.scss',
})
export default class StudyPage {
  /** Bound from the `/study/:deckId` route. */
  readonly deckId = input.required<string>();

  private readonly router = inject(Router);
  private readonly telegram = inject(TelegramService);
  private readonly shell = inject(ShellStore);

  protected readonly store = inject(StudyStore);
  protected readonly learner = inject(LearnerStore);

  private readonly flashcard = viewChild(Flashcard);

  constructor() {
    effect(() => {
      void this.store.start(this.deckId());
    });

    effect(() => this.shell.setChrome(this.store.deckTitle(), true));
  }

  /** Called by the card's swipe gesture and by the two buttons underneath it. */
  protected async onGraded(known: boolean): Promise<void> {
    const finished = await this.store.grade(known);
    this.telegram.notify(finished ? 'success' : known ? 'success' : 'warning');
  }

  protected grade(known: boolean): void {
    this.flashcard()?.commit(known);
  }

  protected onFlip(): void {
    this.telegram.tap();
  }

  protected async finish(): Promise<void> {
    this.telegram.tap();
    await this.router.navigate(['/'], { replaceUrl: true });
  }

  protected async studyAgain(): Promise<void> {
    this.telegram.tap();
    await this.store.start(this.deckId());
  }

  protected async openDeck(): Promise<void> {
    this.telegram.tap();
    await this.router.navigate(['/decks', this.deckId()], { replaceUrl: true });
  }
}
