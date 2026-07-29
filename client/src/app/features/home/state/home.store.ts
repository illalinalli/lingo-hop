import { Injectable, computed, inject, signal } from '@angular/core';
import { ListDecksUseCase } from '../../../application/decks/list-decks.use-case';
import { LoadLearnerProfileUseCase } from '../../../application/learner/load-learner-profile.use-case';
import { Deck, isStudyable } from '../../../domain/models/deck.model';

/**
 * State for the home screen. Provided by the page component, so it is created when the
 * screen opens and thrown away when it closes.
 */
@Injectable()
export class HomeStore {
  private readonly loadLearner = inject(LoadLearnerProfileUseCase);
  private readonly listDecks = inject(ListDecksUseCase);

  private readonly decksState = signal<readonly Deck[]>([]);
  private readonly loadingState = signal(true);
  private readonly failedState = signal(false);

  readonly decks = this.decksState.asReadonly();
  readonly isLoading = this.loadingState.asReadonly();
  readonly hasFailed = this.failedState.asReadonly();

  /** The deck the "Continue learning" tile points at: the newest one with cards in it. */
  readonly featuredDeck = computed(() => this.decksState().find(isStudyable));

  /** Decks other than the featured one, for the list below. */
  readonly otherDecks = computed(() => {
    const featured = this.featuredDeck();
    return this.decksState().filter((deck) => deck.id !== featured?.id);
  });

  readonly hasDecks = computed(() => this.decksState().length > 0);

  async load(): Promise<void> {
    this.loadingState.set(true);
    this.failedState.set(false);

    try {
      // The profile call also registers the learner on first launch, so it must succeed
      // before the deck list is meaningful.
      await this.loadLearner.execute();
      this.decksState.set(await this.listDecks.execute());
    } catch {
      // The HTTP interceptor has already shown the reason as a toast.
      this.failedState.set(true);
    } finally {
      this.loadingState.set(false);
    }
  }
}
