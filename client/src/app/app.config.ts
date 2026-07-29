import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding, withInMemoryScrolling } from '@angular/router';

import { routes } from './app.routes';
import { APP_CONFIG, readRuntimeConfig } from './core/config/app-config';
import { apiErrorInterceptor } from './core/http/api-error.interceptor';
import { telegramAuthInterceptor } from './core/http/telegram-auth.interceptor';
import { DeckRepository } from './domain/ports/deck.repository';
import { LearnerRepository } from './domain/ports/learner.repository';
import { StudyRepository } from './domain/ports/study.repository';
import { HttpDeckRepository } from './infrastructure/http/http-deck.repository';
import { HttpLearnerRepository } from './infrastructure/http/http-learner.repository';
import { HttpStudyRepository } from './infrastructure/http/http-study.repository';

/**
 * Composition root of the front end.
 *
 * This is the only place where the domain's ports are bound to concrete adapters, which is
 * what keeps `domain/` and `application/` free of any knowledge about HTTP.
 */
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),

    provideRouter(
      routes,
      // Route params arrive as component inputs (deckId).
      withComponentInputBinding(),
      withInMemoryScrolling({ scrollPositionRestoration: 'top' }),
    ),

    provideHttpClient(
      withInterceptors([
        // Order matters: attach credentials first, then translate failures.
        telegramAuthInterceptor,
        apiErrorInterceptor,
      ]),
    ),

    { provide: APP_CONFIG, useFactory: readRuntimeConfig },

    // Ports -> adapters.
    { provide: LearnerRepository, useClass: HttpLearnerRepository },
    { provide: DeckRepository, useClass: HttpDeckRepository },
    { provide: StudyRepository, useClass: HttpStudyRepository },
  ],
};
