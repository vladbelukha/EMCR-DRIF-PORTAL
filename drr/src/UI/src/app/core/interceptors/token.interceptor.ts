import { HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import {
  OAuthResourceServerErrorHandler,
  OAuthStorage,
} from 'angular-oauth2-oidc';
import { Observable, catchError } from 'rxjs';

const includedURLs = [/^\/api\/.+$/];
const excludedURLs = [/^\/api\/configuration\/?.*/];

export const TokenInterceptor = (
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> => {
  const oauthStorage = inject(OAuthStorage);
  const errorHandler = inject(OAuthResourceServerErrorHandler);

  if (!includedURLs.some((regexp) => regexp.test(req.url.toLowerCase()))) {
    // all non-API requests are skipped
    return next(req);
  }

  if (excludedURLs.some((regexp) => regexp.test(req.url.toLowerCase()))) {
    // all configuration requests are skipped
    return next(req);
  }

  const token = oauthStorage.getItem('id_token');

  // TODO: improve by checking if authenticated
  if (!token) {
    // TODO: redirect to login page if token is not available?
    console.error('Token is not available for URL:', req.url);
    return next(req);
  }

  const header = 'Bearer ' + token;

  const headers = req.headers.set('Authorization', header);

  req = req.clone({
    headers: headers,
  });

  return next(req).pipe(catchError((err) => errorHandler.handleError(err)));
};
