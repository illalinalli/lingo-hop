/**
 * The slice of the Telegram Mini App SDK (`telegram-web-app.js`) this app uses.
 * Hand-written rather than pulled from a package so there is no dependency to keep in step
 * with the script tag in index.html.
 */
export interface TelegramWebApp {
  /** Signed launch payload. This is what proves the user's identity to our API. */
  readonly initData: string;
  readonly initDataUnsafe?: { user?: TelegramWebAppUser };
  readonly version: string;
  readonly colorScheme: 'light' | 'dark';
  readonly platform: string;

  ready(): void;
  expand(): void;
  close(): void;
  isVersionAtLeast(version: string): boolean;

  setHeaderColor?(color: string): void;
  setBackgroundColor?(color: string): void;
  disableVerticalSwipes?(): void;

  readonly BackButton: {
    show(): void;
    hide(): void;
    onClick(handler: () => void): void;
    offClick(handler: () => void): void;
  };

  readonly HapticFeedback: {
    impactOccurred(style: 'light' | 'medium' | 'heavy' | 'rigid' | 'soft'): void;
    notificationOccurred(type: 'error' | 'success' | 'warning'): void;
    selectionChanged(): void;
  };
}

export interface TelegramWebAppUser {
  readonly id: number;
  readonly first_name: string;
  readonly last_name?: string;
  readonly username?: string;
  readonly language_code?: string;
}

/** Reads the SDK off `window`, returning `undefined` when running in a plain browser. */
export function resolveTelegramWebApp(): TelegramWebApp | undefined {
  return (globalThis as { Telegram?: { WebApp?: TelegramWebApp } }).Telegram?.WebApp;
}
