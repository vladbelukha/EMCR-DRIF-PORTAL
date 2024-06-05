import { Injectable, inject } from '@angular/core';
import { of, switchMap } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationGuard {
  authService = inject(AuthService);

  canActivate() {
    // TODO: maybe just use the authService.isLoggedIn, needs testing
    this.authService.waitUntilAuthentication$.pipe(
      switchMap((isAuthenticated) => of(isAuthenticated))
    );
  }
}
