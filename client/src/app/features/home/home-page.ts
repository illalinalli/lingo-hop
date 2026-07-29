import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { LearnerStore } from '../../core/state/learner.store';
import { TelegramService } from '../../core/telegram/telegram.service';
import { greetingFor } from '../../domain/models/learner.model';
import { EmptyState } from '../../shared/ui/empty-state/empty-state';
import { ProgressBar } from '../../shared/ui/progress-bar/progress-bar';
import { Spinner } from '../../shared/ui/spinner/spinner';
import { StatPill } from '../../shared/ui/stat-pill/stat-pill';
import { DeckTile } from '../decks/ui/deck-tile/deck-tile';
import { ShellStore } from '../shell/state/shell.store';
import { HomeStore } from './state/home.store';

/** The dashboard: who you are, how you are doing, and the way into today's lesson. */
@Component({
  selector: 'lh-home-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DeckTile, EmptyState, ProgressBar, Spinner, StatPill],
  providers: [HomeStore],
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
})
export default class HomePage {
  private readonly router = inject(Router);
  private readonly telegram = inject(TelegramService);

  protected readonly store = inject(HomeStore);
  protected readonly learner = inject(LearnerStore);

  protected readonly greeting = greetingFor();

  /** First letter of the learner's name, standing in for an avatar. */
  protected readonly initial = computed(() =>
    (this.learner.profile()?.firstName ?? '?').charAt(0).toUpperCase(),
  );

  protected readonly goalLabel = computed(() => {
    const profile = this.learner.profile();
    if (!profile) {
      return '';
    }
    return `${Math.min(profile.cardsReviewedToday, profile.dailyGoalCards)} / ${profile.dailyGoalCards} cards`;
  });

  constructor() {
    inject(ShellStore).setChrome('LingoHop', false);
    void this.store.load();
  }

  protected async study(deckId: string): Promise<void> {
    this.telegram.tap();
    await this.router.navigate(['/study', deckId]);
  }

  protected async openDeck(deckId: string): Promise<void> {
    this.telegram.tap();
    await this.router.navigate(['/decks', deckId]);
  }

  protected async createDeck(): Promise<void> {
    this.telegram.tap();
    await this.router.navigate(['/decks/new']);
  }
}
