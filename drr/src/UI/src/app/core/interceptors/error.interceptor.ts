import { HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';

export const ErrorInterceptor = (
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> => {
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((err) => {
      if (err.status === 401) {
        // TODO: maybe API auth have different timeout or smth?
        // authService.logout();
        // authService.login();
      }
      return throwError(err);
    })
  );
};
