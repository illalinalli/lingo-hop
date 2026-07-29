import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  input,
  output,
  signal,
} from '@angular/core';
import { StudyCard } from '../../../../domain/models/study-session.model';

/** Horizontal distance, in pixels, that commits a swipe to a grade. */
const SWIPE_THRESHOLD = 95;

/** Movement beyond this counts as a drag rather than a tap, so it does not flip the card. */
const DRAG_TOLERANCE = 6;

/**
 * The flip-and-swipe flashcard from the design.
 *
 * Tap flips between the word and its meaning; dragging right means "Know" and left means
 * "Don't know". All of the gesture state is local - the component reports one decision and
 * resets whenever a different card arrives.
 */
@Component({
  selector: 'lh-flashcard',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './flashcard.html',
  styleUrl: './flashcard.scss',
})
export class Flashcard {
  readonly card = input.required<StudyCard>();

  /** Blocks input while a grade is in flight. */
  readonly disabled = input(false);

  /**
   * Change this to force the card back to its resting position - the study page bumps it
   * when a grade request failed and the swipe has to be undone.
   */
  readonly revision = input(0);

  /** True for "Know", false for "Don't know". */
  readonly graded = output<boolean>();

  /** Emitted on a tap that flipped the card, for haptics. */
  readonly flipped = output<void>();

  protected readonly isFlipped = signal(false);
  protected readonly dragX = signal(0);
  protected readonly isDragging = signal(false);
  protected readonly flyAway = signal<'know' | 'dont' | null>(null);

  private startX = 0;
  private moved = false;

  /** Where the card sits right now: mid-drag, flying off, or at rest. */
  private readonly offset = computed(() => {
    const fly = this.flyAway();
    if (fly) {
      return fly === 'know' ? 520 : -520;
    }
    return this.dragX();
  });

  protected readonly transform = computed(
    () => `translateX(${this.offset()}px) rotate(${this.offset() / 22}deg)`,
  );

  protected readonly transition = computed(() =>
    this.isDragging() ? 'none' : 'transform .28s cubic-bezier(.34,1.4,.6,1)',
  );

  protected readonly flipTransform = computed(() =>
    this.isFlipped() ? 'rotateY(180deg)' : 'rotateY(0deg)',
  );

  protected readonly knowOpacity = computed(() => this.stampOpacity(this.offset()));

  protected readonly dontKnowOpacity = computed(() => this.stampOpacity(-this.offset()));

  protected readonly partOfSpeechLabel = computed(() => {
    const part = this.card().partOfSpeech;
    return part === 'Unspecified' ? '' : part.toLowerCase();
  });

  constructor() {
    // A new card - or a rolled-back grade - returns the card to its resting state.
    effect(() => {
      this.card().cardId;
      this.revision();

      this.isFlipped.set(false);
      this.dragX.set(0);
      this.isDragging.set(false);
      this.flyAway.set(null);
      this.moved = false;
    });
  }

  protected onPointerDown(event: PointerEvent): void {
    if (this.disabled() || this.flyAway()) {
      return;
    }

    this.startX = event.clientX;
    this.moved = false;
    this.isDragging.set(true);

    // Capture so the gesture keeps tracking even if the finger leaves the card.
    (event.currentTarget as HTMLElement).setPointerCapture?.(event.pointerId);
  }

  protected onPointerMove(event: PointerEvent): void {
    if (!this.isDragging()) {
      return;
    }

    const delta = event.clientX - this.startX;
    if (Math.abs(delta) > DRAG_TOLERANCE) {
      this.moved = true;
    }
    this.dragX.set(delta);
  }

  protected onPointerUp(): void {
    if (!this.isDragging()) {
      return;
    }

    const delta = this.dragX();
    this.isDragging.set(false);

    if (delta > SWIPE_THRESHOLD) {
      this.commit(true);
    } else if (delta < -SWIPE_THRESHOLD) {
      this.commit(false);
    } else {
      this.dragX.set(0);
    }
  }

  /** A tap that was not part of a drag flips the card. */
  protected onClick(): void {
    if (this.disabled() || this.moved || this.flyAway()) {
      return;
    }

    this.isFlipped.update((flipped) => !flipped);
    this.flipped.emit();
  }

  /** Used by the Know / Don't know buttons under the card. */
  commit(known: boolean): void {
    if (this.disabled() || this.flyAway()) {
      return;
    }

    this.flyAway.set(known ? 'know' : 'dont');
    this.graded.emit(known);
  }

  private stampOpacity(distance: number): number {
    return distance > 25 ? Math.min(1, (distance - 25) / 70) : 0;
  }
}
