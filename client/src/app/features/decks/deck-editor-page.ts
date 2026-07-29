import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ToastService } from '../../core/notifications/toast.service';
import { TelegramService } from '../../core/telegram/telegram.service';
import { CardDraft } from '../../domain/models/deck.model';
import { EmptyState } from '../../shared/ui/empty-state/empty-state';
import { ProgressBar } from '../../shared/ui/progress-bar/progress-bar';
import { Spinner } from '../../shared/ui/spinner/spinner';
import { ShellStore } from '../shell/state/shell.store';
import { DeckEditorStore } from './state/deck-editor.store';
import { CardForm } from './ui/card-form/card-form';
import { CardRow } from './ui/card-row/card-row';

/** Emoji offered as deck badges; the learner can also type any character. */
const ICON_CHOICES = ['📘', '☕', '✈️', '🍽️', '💼', '🏥', '🛒', '🎬', '⚽', '🏠'] as const;

/**
 * Creates a deck and manages its cards.
 *
 * One component serves both modes: with no `deckId` it creates, and after creating it
 * navigates to the saved deck so the very same screen becomes the editor.
 */
@Component({
  selector: 'lh-deck-editor-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CardForm, CardRow, EmptyState, ProgressBar, Spinner],
  providers: [DeckEditorStore],
  templateUrl: './deck-editor-page.html',
  styleUrl: './deck-editor-page.scss',
})
export default class DeckEditorPage {
  /** Bound from the route. Absent on `/decks/new`, which puts the screen in create mode. */
  readonly deckId = input<string | undefined>(undefined);

  private readonly router = inject(Router);
  private readonly telegram = inject(TelegramService);
  private readonly toasts = inject(ToastService);
  private readonly shell = inject(ShellStore);

  protected readonly store = inject(DeckEditorStore);

  protected readonly iconChoices = ICON_CHOICES;

  protected readonly title = signal('');
  protected readonly icon = signal<string>(ICON_CHOICES[0]);
  protected readonly editingCardId = signal<string | null>(null);
  protected readonly confirmingDelete = signal(false);

  protected readonly isCreateMode = computed(() => this.deckId() === undefined);

  protected readonly canSaveDeck = computed(
    () => this.title().trim().length > 0 && !this.store.isBusy(),
  );

  protected readonly completionPercent = computed(() =>
    Math.round((this.store.deck()?.completion ?? 0) * 100),
  );

  protected readonly editingCard = computed(() => {
    const cardId = this.editingCardId();
    return cardId ? this.store.cards().find((card) => card.id === cardId) : undefined;
  });

  protected readonly editingDraft = computed<CardDraft | undefined>(() => {
    const card = this.editingCard();
    return card
      ? {
          term: card.term,
          translation: card.translation,
          partOfSpeech: card.partOfSpeech,
          example: card.example,
        }
      : undefined;
  });

  constructor() {
    // Load whenever the route id changes, including the switch from create to edit.
    effect(() => {
      const deckId = this.deckId();
      if (deckId) {
        void this.store.load(deckId);
      }
    });

    // Keep the header and the name field in step with whatever the server last returned.
    effect(() => {
      const deck = this.store.deck();
      if (deck) {
        this.title.set(deck.title);
        this.icon.set(deck.icon);
        this.shell.setChrome(deck.title, true);
      } else if (this.isCreateMode()) {
        this.shell.setChrome('New deck', true);
      }
    });
  }

  protected onTitle(value: string): void {
    this.title.set(value);
  }

  protected chooseIcon(icon: string): void {
    this.telegram.tap();
    this.icon.set(icon);
  }

  protected async saveDeck(): Promise<void> {
    if (!this.canSaveDeck()) {
      return;
    }

    if (this.isCreateMode()) {
      const createdId = await this.store.create(this.title().trim(), this.icon(), []);
      if (createdId) {
        // Replace the URL so Back does not return to an empty "new deck" form.
        await this.router.navigate(['/decks', createdId], { replaceUrl: true });
      }
      return;
    }

    await this.store.rename(this.title().trim(), this.icon());
  }

  protected async addCard(draft: CardDraft): Promise<void> {
    const added = await this.store.addCard(draft);
    this.telegram.notify(added ? 'success' : 'error');
  }

  protected async saveCard(draft: CardDraft): Promise<void> {
    const cardId = this.editingCardId();
    if (!cardId) {
      return;
    }

    if (await this.store.updateCard(cardId, draft)) {
      this.editingCardId.set(null);
    }
  }

  protected startEditing(cardId: string): void {
    this.telegram.tap();
    this.editingCardId.set(cardId);
  }

  protected cancelEditing(): void {
    this.editingCardId.set(null);
  }

  protected async removeCard(cardId: string): Promise<void> {
    this.telegram.tap();
    if (this.editingCardId() === cardId) {
      this.editingCardId.set(null);
    }
    await this.store.removeCard(cardId);
  }

  protected async study(): Promise<void> {
    const deckId = this.store.deck()?.id;
    if (!deckId) {
      return;
    }

    this.telegram.tap();
    await this.router.navigate(['/study', deckId]);
  }

  protected async resetProgress(): Promise<void> {
    this.telegram.tap();
    await this.store.reset();
  }

  /** First tap arms the delete, second one performs it. Avoids a native confirm dialog. */
  protected async deleteDeck(): Promise<void> {
    this.telegram.tap();

    if (!this.confirmingDelete()) {
      this.confirmingDelete.set(true);
      this.toasts.show('Tap again to delete this deck');
      return;
    }

    if (await this.store.remove()) {
      await this.router.navigate(['/decks'], { replaceUrl: true });
    } else {
      this.confirmingDelete.set(false);
    }
  }
}
