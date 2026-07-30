import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { Deck, completionPercent } from '../../../../domain/models/deck.model';
import { ProgressBar } from '../../../../shared/ui/progress-bar/progress-bar';

/**
 * The deck card from the design: emoji badge, title, card count and a completion bar.
 * Purely presentational - it reports taps and knows nothing about routing or loading.
 */
@Component({
  selector: 'lh-deck-tile',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ProgressBar],
  template: `
    <button type="button" class="tile" (click)="select.emit(deck().id)">
      <span class="badge" aria-hidden="true">{{ deck().icon }}</span>

      <span class="body">
        <span class="top">
          <span class="title">{{ deck().title }}</span>
          @if (showBadge()) {
            <span class="chip">{{ percent() }}%</span>
          }
        </span>

        <span class="meta">{{ subtitle() }}</span>

        <lh-progress-bar
          [value]="deck().completion"
          [label]="deck().title + ' progress'"
        />
      </span>
    </button>
  `,
  styleUrl: './deck-tile.scss',
})
export class DeckTile {
  readonly deck = input.required<Deck>();

  /** Shows the percentage chip; off in dense lists. */
  readonly showBadge = input(true);

  readonly select = output<string>();

  protected readonly percent = computed(() => completionPercent(this.deck()));

  protected readonly subtitle = computed(() => {
    const deck = this.deck();
    if (deck.cardCount === 0) {
      return 'No cards yet - tap to add some';
    }
    // Counts what the bar counts, so the number and the percentage never disagree.
    return `${deck.cardCount} ${deck.cardCount === 1 ? 'word' : 'words'} · ${deck.knownCardCount} known`;
  });
}
