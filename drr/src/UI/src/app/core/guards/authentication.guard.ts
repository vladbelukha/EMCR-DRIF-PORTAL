import { Injectable, inject } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { of, switchMap } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { ProfileStore } from '../../store/profile.store';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationGuard {
  authService = inject(AuthService);
  profileStore = inject(ProfileStore);

  async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (!this.profileStore.loggedIn()) {
      await this.authService.init({});
    }
    
    const isAuthenticated = this.authService.waitUntilAuthentication$.pipe(
      switchMap((isAuthenticated) =>
        isAuthenticated ? of(this.profileStore.loggedIn()) : of(isAuthenticated)
      )
    );

    return isAuthenticated.pipe(
      take(1),
      map((isAuthenticated) => {
        return isAuthenticated;
      })
    );
  }
}
