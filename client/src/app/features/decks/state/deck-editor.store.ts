import { Injectable, computed, inject, signal } from '@angular/core';
import { AddCardUseCase } from '../../../application/decks/add-card.use-case';
import { CreateDeckUseCase } from '../../../application/decks/create-deck.use-case';
import { DeleteCardUseCase } from '../../../application/decks/delete-card.use-case';
import { DeleteDeckUseCase } from '../../../application/decks/delete-deck.use-case';
import { GetDeckUseCase } from '../../../application/decks/get-deck.use-case';
import { RenameDeckUseCase } from '../../../application/decks/rename-deck.use-case';
import { ResetDeckProgressUseCase } from '../../../application/decks/reset-deck-progress.use-case';
import { UpdateCardUseCase } from '../../../application/decks/update-card.use-case';
import { ToastService } from '../../../core/notifications/toast.service';
import { CardDraft, DeckDetails } from '../../../domain/models/deck.model';

/**
 * State for the deck editor, which serves both "new deck" and "edit deck".
 *
 * Every mutation returns the server's version of the deck, so the counters shown here
 * (cards, learned, completion) always match what the API computed rather than a local guess.
 */
@Injectable()
export class DeckEditorStore {
  private readonly getDeck = inject(GetDeckUseCase);
  private readonly createDeck = inject(CreateDeckUseCase);
  private readonly renameDeck = inject(RenameDeckUseCase);
  private readonly deleteDeck = inject(DeleteDeckUseCase);
  private readonly resetProgress = inject(ResetDeckProgressUseCase);
  private readonly addCardUseCase = inject(AddCardUseCase);
  private readonly updateCardUseCase = inject(UpdateCardUseCase);
  private readonly deleteCardUseCase = inject(DeleteCardUseCase);
  private readonly toasts = inject(ToastService);

  private readonly deckState = signal<DeckDetails | null>(null);
  private readonly loadingState = signal(false);
  private readonly busyState = signal(false);

  readonly deck = this.deckState.asReadonly();
  readonly isLoading = this.loadingState.asReadonly();
  /** True while a mutation is in flight, so the UI can disable its buttons. */
  readonly isBusy = this.busyState.asReadonly();

  readonly isPersisted = computed(() => this.deckState() !== null);
  readonly cards = computed(() => this.deckState()?.cards ?? []);
  readonly canStudy = computed(() => (this.deckState()?.cardCount ?? 0) > 0);

  async load(deckId: string): Promise<void> {
    this.loadingState.set(true);
    try {
      this.deckState.set(await this.getDeck.execute(deckId));
    } catch {
      this.deckState.set(null);
    } finally {
      this.loadingState.set(false);
    }
  }

  /** Creates the deck. Returns its id, or `null` when the request failed. */
  async create(title: string, icon: string, cards: readonly CardDraft[]): Promise<string | null> {
    return this.run(async () => {
      const deck = await this.createDeck.execute({ title, icon, cards });
      this.deckState.set(deck);
      this.toasts.success('Deck created');
      return deck.id;
    });
  }

  async rename(title: string, icon: string): Promise<void> {
    const deckId = this.deckState()?.id;
    if (!deckId) {
      return;
    }

    await this.run(async () => {
      this.deckState.set(await this.renameDeck.execute(deckId, title, icon));
      this.toasts.success('Deck updated');
    });
  }

  async addCard(draft: CardDraft): Promise<boolean> {
    const deckId = this.deckState()?.id;
    if (!deckId) {
      return false;
    }

    const result = await this.run(async () => {
      await this.addCardUseCase.execute(deckId, draft);
      // Re-read so cardCount and completion come from the server.
      this.deckState.set(await this.getDeck.execute(deckId));
      this.toasts.success(`"${draft.term}" added`);
      return true;
    });

    return result ?? false;
  }

  async updateCard(cardId: string, draft: CardDraft): Promise<boolean> {
    const deckId = this.deckState()?.id;
    if (!deckId) {
      return false;
    }

    const result = await this.run(async () => {
      await this.updateCardUseCase.execute(deckId, cardId, draft);
      this.deckState.set(await this.getDeck.execute(deckId));
      this.toasts.success('Card updated');
      return true;
    });

    return result ?? false;
  }

  async removeCard(cardId: string): Promise<void> {
    const deckId = this.deckState()?.id;
    if (!deckId) {
      return;
    }

    await this.run(async () => {
      await this.deleteCardUseCase.execute(deckId, cardId);
      this.deckState.set(await this.getDeck.execute(deckId));
    });
  }

  async reset(): Promise<void> {
    const deckId = this.deckState()?.id;
    if (!deckId) {
      return;
    }

    await this.run(async () => {
      this.deckState.set(await this.resetProgress.execute(deckId));
      this.toasts.success('Progress reset');
    });
  }

  /** Returns true when the deck was deleted. */
  async remove(): Promise<boolean> {
    const deckId = this.deckState()?.id;
    if (!deckId) {
      return false;
    }

    const result = await this.run(async () => {
      await this.deleteDeck.execute(deckId);
      this.deckState.set(null);
      return true;
    });

    return result ?? false;
  }

  /** Runs a mutation with the busy flag set, swallowing errors the interceptor reported. */
  private async run<T>(operation: () => Promise<T>): Promise<T | null> {
    if (this.busyState()) {
      return null;
    }

    this.busyState.set(true);
    try {
      return await operation();
    } catch {
      return null;
    } finally {
      this.busyState.set(false);
    }
  }
}
