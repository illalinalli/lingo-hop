import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

/** Placeholder shown when a list has nothing in it yet. */
@Component({
  selector: 'lh-empty-state',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="empty">
      <div class="icon" aria-hidden="true">{{ icon() }}</div>
      <h2 class="title">{{ title() }}</h2>
      <p class="message">{{ message() }}</p>
      @if (actionLabel()) {
        <button type="button" class="action" (click)="action.emit()">{{ actionLabel() }}</button>
      }
    </div>
  `,
  styles: `
    .empty {
      text-align: center;
      padding: 34px 20px;
      background: var(--lh-surface);
      border-radius: var(--lh-radius-xl);
      box-shadow: var(--lh-shadow-card);
    }

    .icon {
      font-size: 40px;
    }

    .title {
      margin: 10px 0 4px;
      font-size: 18px;
      font-weight: 900;
      color: var(--lh-ink);
    }

    .message {
      margin: 0;
      font-size: 14px;
      font-weight: 600;
      color: var(--lh-muted);
      line-height: 1.5;
    }

    .action {
      margin-top: 18px;
      width: 100%;
      border: none;
      border-radius: var(--lh-radius-lg);
      padding: 15px;
      font-size: 16px;
      font-weight: 900;
      color: #fff;
      background: var(--lh-green-gradient);
      box-shadow: var(--lh-shadow-green);
    }
  `,
})
export class EmptyState {
  readonly icon = input('📚');

  readonly title = input.required<string>();

  readonly message = input.required<string>();

  readonly actionLabel = input<string | undefined>(undefined);

  readonly action = output<void>();
}
