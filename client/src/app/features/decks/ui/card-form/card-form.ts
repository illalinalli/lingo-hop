import { ChangeDetectionStrategy, Component, computed, effect, input, output, signal } from '@angular/core';
import {
  CardDraft,
  PARTS_OF_SPEECH,
  PartOfSpeech,
} from '../../../../domain/models/deck.model';

/**
 * Add/edit form for one card. Uncontrolled by design: it keeps its own draft in signals and
 * emits a finished {@link CardDraft}, so the parent never has to track keystrokes.
 */
@Component({
  selector: 'lh-card-form',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './card-form.html',
  styleUrl: './card-form.scss',
})
export class CardForm {
  /** Existing card to edit; omit to add a new one. */
  readonly initial = input<CardDraft | undefined>(undefined);

  readonly submitLabel = input('Add card');

  readonly disabled = input(false);

  /** Shows a cancel button, for edit mode. */
  readonly cancellable = input(false);

  readonly save = output<CardDraft>();

  readonly cancel = output<void>();

  protected readonly partsOfSpeech = PARTS_OF_SPEECH;

  protected readonly term = signal('');
  protected readonly translation = signal('');
  protected readonly partOfSpeech = signal<PartOfSpeech>('Unspecified');
  protected readonly example = signal('');

  protected readonly canSave = computed(
    () => !this.disabled() && this.term().trim().length > 0 && this.translation().trim().length > 0,
  );

  constructor() {
    // Load the card being edited into the draft, and reset when switching to add mode.
    effect(() => {
      const initial = this.initial();
      this.term.set(initial?.term ?? '');
      this.translation.set(initial?.translation ?? '');
      this.partOfSpeech.set(initial?.partOfSpeech ?? 'Unspecified');
      this.example.set(initial?.example ?? '');
    });
  }

  protected submit(): void {
    if (!this.canSave()) {
      return;
    }

    this.save.emit({
      term: this.term().trim(),
      translation: this.translation().trim(),
      partOfSpeech: this.partOfSpeech(),
      example: this.example().trim() || undefined,
    });

    // Only add mode clears itself; edit mode is closed by the parent.
    if (!this.initial()) {
      this.reset();
    }
  }

  protected reset(): void {
    this.term.set('');
    this.translation.set('');
    this.partOfSpeech.set('Unspecified');
    this.example.set('');
  }

  protected onTerm(value: string): void {
    this.term.set(value);
  }

  protected onTranslation(value: string): void {
    this.translation.set(value);
  }

  protected onExample(value: string): void {
    this.example.set(value);
  }

  protected onPartOfSpeech(value: string): void {
    this.partOfSpeech.set(value as PartOfSpeech);
  }
}
