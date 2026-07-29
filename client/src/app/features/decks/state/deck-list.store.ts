import { Injectable, computed, inject, signal } from '@angular/core';
import { ListDecksUseCase } from '../../../application/decks/list-decks.use-case';
import { Deck } from '../../../domain/models/deck.model';

/** State for the deck management screen. */
@Injectable()
export class DeckListStore {
  private readonly listDecks = inject(ListDecksUseCase);

  private readonly decksState = signal<readonly Deck[]>([]);
  private readonly loadingState = signal(true);

  readonly decks = this.decksState.asReadonly();
  readonly isLoading = this.loadingState.asReadonly();
  readonly isEmpty = computed(() => !this.loadingState() && this.decksState().length === 0);

  readonly totalCards = computed(() =>
    this.decksState().reduce((sum, deck) => sum + deck.cardCount, 0),
  );

  async load(): Promise<void> {
    this.loadingState.set(true);
    try {
      this.decksState.set(await this.listDecks.execute());
    } catch {
      // Already surfaced by the HTTP error interceptor.
    } finally {
      this.loadingState.set(false);
    }
  }
}
