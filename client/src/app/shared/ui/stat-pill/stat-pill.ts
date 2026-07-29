import { ChangeDetectionStrategy, Component, input } from '@angular/core';

/** The small white pill holding an emoji and a number, e.g. "🔥 7". */
@Component({
  selector: 'lh-stat-pill',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <span class="pill" [attr.aria-label]="label()">
      <span class="icon" aria-hidden="true">{{ icon() }}</span>
      <span class="value">{{ value() }}</span>
    </span>
  `,
  styles: `
    .pill {
      display: inline-flex;
      align-items: center;
      gap: 5px;
      background: var(--lh-surface);
      padding: 8px 12px;
      border-radius: 16px;
      box-shadow: 0 2px 6px rgba(80, 60, 40, 0.06);
    }

    .icon {
      font-size: 15px;
      line-height: 1;
    }

    .value {
      font-weight: 900;
      font-size: 16px;
      color: var(--lh-ink);
    }
  `,
})
export class StatPill {
  readonly icon = input.required<string>();

  readonly value = input.required<string | number>();

  readonly label = input.required<string>();
}
