import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { TelegramService } from '../telegram/telegram.service';

/**
 * Attaches the Telegram launch payload to every API call as
 * `Authorization: tma <initData>` - the scheme the API's authentication handler expects.
 *
 * There is no token exchange and nothing to refresh: the signed payload *is* the credential,
 * and the server re-verifies its HMAC on each request.
 *
 * Outside Telegram no header is sent, and the API's development fallback attributes the
 * request to a local test learner.
 */
export const telegramAuthInterceptor: HttpInterceptorFn = (request, next) => {
  const initData = inject(TelegramService).initData;

  if (!initData) {
    return next(request);
  }

  return next(
    request.clone({
      setHeaders: { Authorization: `tma ${initData}` },
    }),
  );
};
