import { Injectable, signal } from '@angular/core';

export type ToastTone = 'neutral' | 'success' | 'error';

export interface Toast {
  readonly message: string;
  readonly tone: ToastTone;
}

/**
 * The dark pill that slides up from the bottom of the mock-up. One at a time; a new toast
 * replaces the current one and restarts the timer.
 */
@Injectable({ providedIn: 'root' })
export class ToastService {
  private static readonly visibleForMs = 2200;

  private readonly current = signal<Toast | null>(null);
  private timer: ReturnType<typeof setTimeout> | undefined;

  readonly toast = this.current.asReadonly();

  show(message: string, tone: ToastTone = 'neutral'): void {
    clearTimeout(this.timer);
    this.current.set({ message, tone });
    this.timer = setTimeout(() => this.current.set(null), ToastService.visibleForMs);
  }

  success(message: string): void {
    this.show(message, 'success');
  }

  error(message: string): void {
    this.show(message, 'error');
  }

  dismiss(): void {
    clearTimeout(this.timer);
    this.current.set(null);
  }
}
