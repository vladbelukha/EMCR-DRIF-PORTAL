import { Injectable, inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable, of, switchMap } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { ProfileStore } from '../../store/profile.store';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationGuard {
  authService = inject(AuthService);
  profileStore = inject(ProfileStore);

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ):
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
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
