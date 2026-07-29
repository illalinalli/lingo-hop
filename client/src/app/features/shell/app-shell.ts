import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { filter, map, startWith } from 'rxjs';
import { TelegramService } from '../../core/telegram/telegram.service';
import { ToastHost } from '../../shared/ui/toast-host/toast-host';
import { ShellStore } from './state/shell.store';

interface NavTab {
  readonly path: string;
  readonly label: string;
  readonly icon: string;
}

/**
 * The phone frame from the design: a header, the routed screen, and the bottom tab bar.
 * Also bridges Telegram's native back button to the Angular router.
 */
@Component({
  selector: 'lh-app-shell',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, RouterLink, ToastHost],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss',
})
export class AppShell {
  private readonly router = inject(Router);
  private readonly telegram = inject(TelegramService);
  private readonly shell = inject(ShellStore);

  protected readonly title = this.shell.title;
  protected readonly canGoBack = this.shell.canGoBack;

  protected readonly tabs: readonly NavTab[] = [
    { path: '/', label: 'Home', icon: '🏠' },
    { path: '/decks', label: 'Decks', icon: '🗂️' },
    { path: '/profile', label: 'Profile', icon: '📈' },
  ];

  private readonly url = toSignal(
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      map((event) => event.urlAfterRedirects),
      startWith(this.router.url),
    ),
    { initialValue: this.router.url },
  );

  /** A tab is active for its own URL and, for Decks, any deck detail below it. */
  protected readonly activeTab = computed(() => {
    const url = this.url().split('?')[0];
    if (url.startsWith('/decks')) {
      return '/decks';
    }
    if (url.startsWith('/profile')) {
      return '/profile';
    }
    return '/';
  });

  private readonly onTelegramBack = () => void this.goBack();

  constructor() {
    this.telegram.initialise();
    this.telegram.showBackButton(this.onTelegramBack);

    inject(DestroyRef).onDestroy(() => this.telegram.hideBackButton(this.onTelegramBack));
  }

  protected async goBack(): Promise<void> {
    this.telegram.tap();

    // Deck and study screens sit under a tab; falling back to Home keeps the mini app from
    // dead-ending when it was opened straight onto a detail URL.
    if (this.canGoBack()) {
      await this.router.navigateByUrl('/');
    }
  }

  protected onTabTap(): void {
    this.telegram.tap();
  }
}
