import { Injectable, signal } from '@angular/core';

/**
 * What the shell chrome should show for the screen currently on top: its title, and whether
 * the header's back affordance applies. Pages set this when they load.
 */
@Injectable({ providedIn: 'root' })
export class ShellStore {
  private readonly titleState = signal('LingoHop');
  private readonly backState = signal(false);

  readonly title = this.titleState.asReadonly();
  readonly canGoBack = this.backState.asReadonly();

  /** @param canGoBack whether this screen is a detail view the learner can back out of. */
  setChrome(title: string, canGoBack: boolean): void {
    this.titleState.set(title);
    this.backState.set(canGoBack);
  }
}
