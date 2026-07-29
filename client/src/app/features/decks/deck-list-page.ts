import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TelegramService } from '../../core/telegram/telegram.service';
import { EmptyState } from '../../shared/ui/empty-state/empty-state';
import { Spinner } from '../../shared/ui/spinner/spinner';
import { ShellStore } from '../shell/state/shell.store';
import { DeckListStore } from './state/deck-list.store';
import { DeckTile } from './ui/deck-tile/deck-tile';

/** Deck management: everything the learner has, plus the way to add another. */
@Component({
  selector: 'lh-deck-list-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DeckTile, EmptyState, Spinner],
  providers: [DeckListStore],
  templateUrl: './deck-list-page.html',
  styleUrl: './deck-list-page.scss',
})
export default class DeckListPage {
  private readonly router = inject(Router);
  private readonly telegram = inject(TelegramService);

  protected readonly store = inject(DeckListStore);

  constructor() {
    inject(ShellStore).setChrome('Your decks', false);
    void this.store.load();
  }

  protected async open(deckId: string): Promise<void> {
    this.telegram.tap();
    await this.router.navigate(['/decks', deckId]);
  }

  protected async create(): Promise<void> {
    this.telegram.tap();
    await this.router.navigate(['/decks/new']);
  }
}
