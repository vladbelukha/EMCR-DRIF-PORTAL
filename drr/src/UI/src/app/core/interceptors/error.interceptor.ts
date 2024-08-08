import { HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { HotToastService } from '@ngneat/hot-toast';
import { Observable, catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';

export const ErrorInterceptor = (
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> => {
  const authService = inject(AuthService);
  const hotToast = inject(HotToastService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((err) => {
      switch (err.status) {
        // case 401:
        //   hotToast.error(
        //     'You are not authenticated. Please try logging in again.'
        //   );
        //   break;
        case 403:
          router.navigate(['/']);
          hotToast.error('You do not have permission to access this resource.');
          break;
        default:
          break;
      }
      return throwError(err);
    })
  );
};
