import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { Card } from '../../../../domain/models/deck.model';

/** One card in the editor's list, with its mastery state and row actions. */
@Component({
  selector: 'lh-card-row',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @let item = card();

    <div class="row" [class.learned]="item.isLearned">
      <div class="text">
        <div class="term-line">
          <span class="term">{{ item.term }}</span>
          @if (item.partOfSpeech !== 'Unspecified') {
            <span class="pos">{{ item.partOfSpeech.toLowerCase() }}</span>
          }
          @if (item.isLearned) {
            <span class="learned-chip" aria-label="Learned">✓ learned</span>
          }
        </div>

        <div class="translation">{{ item.translation }}</div>

        @if (item.example) {
          <div class="example">"{{ item.example }}"</div>
        }

        @if (item.timesSeen > 0) {
          <div class="stats">{{ stats() }}</div>
        }
      </div>

      <div class="actions">
        <button type="button" class="icon" (click)="edit.emit(item.id)" [attr.aria-label]="'Edit ' + item.term">
          ✏️
        </button>
        <button type="button" class="icon" (click)="remove.emit(item.id)" [attr.aria-label]="'Delete ' + item.term">
          🗑️
        </button>
      </div>
    </div>
  `,
  styleUrl: './card-row.scss',
})
export class CardRow {
  readonly card = input.required<Card>();

  readonly edit = output<string>();

  readonly remove = output<string>();

  protected readonly stats = computed(() => {
    const card = this.card();
    const accuracy = Math.round(card.accuracy * 100);
    return `seen ${card.timesSeen}× · ${accuracy}% correct · streak ${card.correctStreak}`;
  });
}
