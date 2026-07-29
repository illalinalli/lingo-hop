import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ToastService } from '../../../core/notifications/toast.service';

/** Renders the single active toast. Mounted once, by the shell. */
@Component({
  selector: 'lh-toast-host',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @let current = toast();
    @if (current) {
      <div class="toast" [class.success]="current.tone === 'success'" [class.error]="current.tone === 'error'" role="status" aria-live="polite">
        {{ current.message }}
      </div>
    }
  `,
  styles: `
    .toast {
      position: fixed;
      left: 50%;
      bottom: calc(90px + var(--lh-safe-bottom));
      transform: translateX(-50%);
      max-width: min(88vw, 420px);
      background: var(--lh-ink);
      color: #fff;
      font-size: 14px;
      font-weight: 800;
      text-align: center;
      padding: 11px 20px;
      border-radius: 16px;
      box-shadow: 0 8px 20px rgba(0, 0, 0, 0.25);
      animation: lh-toast-in 0.25s ease;
      z-index: 60;
    }

    .toast.success {
      background: #3f6b4a;
    }

    .toast.error {
      background: #8f4a4a;
    }
  `,
})
export class ToastHost {
  protected readonly toast = inject(ToastService).toast;
}
