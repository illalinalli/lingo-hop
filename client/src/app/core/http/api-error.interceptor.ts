import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../notifications/toast.service';

/** The ProblemDetails shape the API returns for handled failures. */
interface ProblemDetails {
  readonly title?: string;
  readonly detail?: string;
  readonly code?: string;
}

/**
 * Surfaces API failures as a toast and re-throws, so features can still react but never
 * have to write their own error copy.
 */
export const apiErrorInterceptor: HttpInterceptorFn = (request, next) => {
  const toasts = inject(ToastService);

  return next(request).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse) {
        toasts.error(describe(error));
      }
      return throwError(() => error);
    }),
  );
};

function describe(error: HttpErrorResponse): string {
  // Status 0 means the request never reached the server.
  if (error.status === 0) {
    return 'No connection to LingoHop. Check your network and try again.';
  }

  if (error.status === 401) {
    return 'Telegram could not confirm who you are. Reopen the app from your chat.';
  }

  const problem = error.error as ProblemDetails | string | null;
  if (problem && typeof problem === 'object' && problem.detail) {
    return problem.detail;
  }

  if (error.status === 404) {
    return 'That is not there any more.';
  }

  return 'Something went wrong. Please try again.';
}
