import { InjectionToken } from '@angular/core';

/** Settings resolved at runtime rather than baked into the bundle. */
export interface LingoHopConfig {
  /**
   * Origin of the API. Empty means "same origin", which is the recommended production
   * setup (nginx serves the app and proxies `/api` to Kestrel) and works in development
   * through the Angular dev-server proxy.
   */
  readonly apiBaseUrl: string;
}

export const APP_CONFIG = new InjectionToken<LingoHopConfig>('LINGOHOP_CONFIG');

/**
 * Reads `window.lingoHopConfig`, injected by a small inline script in index.html.
 * Keeping it out of the bundle means one artefact can be deployed to any environment.
 */
export function readRuntimeConfig(): LingoHopConfig {
  const raw = (globalThis as { lingoHopConfig?: Partial<LingoHopConfig> }).lingoHopConfig;
  return { apiBaseUrl: (raw?.apiBaseUrl ?? '').replace(/\/$/, '') };
}
