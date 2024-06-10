import { HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import {
  OAuthResourceServerErrorHandler,
  OAuthStorage,
} from 'angular-oauth2-oidc';
import { Observable, catchError } from 'rxjs';

export const TokenInterceptor = (
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> => {
  const oauthStorage = inject(OAuthStorage);
  const errorHandler = inject(OAuthResourceServerErrorHandler);

  const excludeUrls = ['/assets/'];
  if (excludeUrls.some((url) => req.url.includes(url))) {
    return next(req);
  }

  const token = oauthStorage.getItem('id_token');

  // TODO: redirect to login page if token is not available
  if (!token) {
    console.error('Token is not available');
    return next(req);
  }

  const header = 'Bearer ' + token;

  const headers = req.headers.set('Authorization', header);

  req = req.clone({
    headers: headers,
  });

  return next(req).pipe(catchError((err) => errorHandler.handleError(err)));
};
