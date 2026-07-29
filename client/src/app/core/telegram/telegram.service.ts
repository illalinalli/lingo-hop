import { Injectable } from '@angular/core';
import { TelegramWebApp, resolveTelegramWebApp } from './telegram-web-app.types';

/**
 * Thin wrapper over the Telegram Mini App SDK.
 *
 * Every method is a no-op when the app runs outside Telegram, so the same build works in a
 * plain browser during development (the API's development fallback then supplies the user).
 */
@Injectable({ providedIn: 'root' })
export class TelegramService {
  private readonly webApp: TelegramWebApp | undefined = resolveTelegramWebApp();

  /** Whether we are actually running inside Telegram. */
  get isAvailable(): boolean {
    return this.webApp !== undefined;
  }

  /** The signed launch payload, or an empty string outside Telegram. */
  get initData(): string {
    return this.webApp?.initData ?? '';
  }

  get colorScheme(): 'light' | 'dark' {
    return this.webApp?.colorScheme ?? 'light';
  }

  /** Tells Telegram the UI is painted and claims the full height of the sheet. */
  initialise(): void {
    const webApp = this.webApp;
    if (!webApp) {
      return;
    }

    webApp.ready();
    webApp.expand();

    // Match the mock-up's parchment background so the Telegram chrome blends in.
    webApp.setHeaderColor?.('#f4efe9');
    webApp.setBackgroundColor?.('#f4efe9');

    // Card swiping conflicts with Telegram's pull-to-close gesture.
    webApp.disableVerticalSwipes?.();
  }

  /** Shows Telegram's native back button and routes taps to `handler`. */
  showBackButton(handler: () => void): void {
    const backButton = this.webApp?.BackButton;
    if (!backButton) {
      return;
    }

    backButton.onClick(handler);
    backButton.show();
  }

  hideBackButton(handler: () => void): void {
    const backButton = this.webApp?.BackButton;
    if (!backButton) {
      return;
    }

    backButton.offClick(handler);
    backButton.hide();
  }

  /** Light tap, used when flipping a card or tapping a tile. */
  tap(): void {
    this.webApp?.HapticFeedback.impactOccurred('light');
  }

  /** Success/error buzz, used when a card is graded or a lesson completes. */
  notify(type: 'success' | 'error' | 'warning'): void {
    this.webApp?.HapticFeedback.notificationOccurred(type);
  }

  close(): void {
    this.webApp?.close();
  }
}
