import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

/** The rounded green bar used for deck completion, lesson progress and the level meter. */
@Component({
  selector: 'lh-progress-bar',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div
      class="track"
      [style.height.px]="height()"
      role="progressbar"
      aria-valuemin="0"
      aria-valuemax="100"
      [attr.aria-valuenow]="percent()"
      [attr.aria-label]="label()"
    >
      <div class="fill" [style.width.%]="percent()"></div>
    </div>
  `,
  styles: `
    .track {
      width: 100%;
      border-radius: 6px;
      background: var(--lh-line);
      overflow: hidden;
    }

    .fill {
      height: 100%;
      border-radius: 6px;
      background: var(--lh-green-bar);
      transition: width 0.45s ease;
    }
  `,
})
export class ProgressBar {
  /** Progress as a fraction, 0..1. Values outside the range are clamped. */
  readonly value = input.required<number>();

  readonly height = input(8);

  readonly label = input('Progress');

  protected readonly percent = computed(() => Math.round(Math.min(1, Math.max(0, this.value())) * 100));
}
