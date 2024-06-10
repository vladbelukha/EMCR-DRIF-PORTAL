import { Injectable, inject, signal } from '@angular/core';
import { AuthConfig, OAuthEvent, OAuthService } from 'angular-oauth2-oidc';
import { Subject } from 'rxjs';
import { ProfileStore } from '../../store/profile.store';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  oauthService = inject(OAuthService);
  profileStore = inject(ProfileStore);
  // platformId = inject(PLATFORM_ID);
  // windowRef = inject(WindowRef);

  private _authConfig: AuthConfig = {
    issuer: 'https://dev.loginproxy.gov.bc.ca/auth/realms/standard', // TODO: change to real issuer
    
    clientId: '', // TODO: change to real client id
    responseType: 'code',
    strictDiscoveryDocumentValidation: false,
    scope: 'openid profile email offline_access',
    showDebugInformation: false,
    requireHttps: true,
    redirectUri: 'http://localhost:4200/', // TODO: it supposed to be window.location.origin, but it's not working because of SSR
    openUri: (uri: string) => {
      const url = new URL(uri);

      url.searchParams.delete('id_token_hint');
      // reconstruct url
      const reconstructUri = `${url.origin}${
        url.pathname
      }?${url.searchParams.toString()}`;

      location.href = reconstructUri;
    },
  };

  isDoneLoading = signal(false);

  isAuthenticationSuccessful = signal(false);

  private _waitUntilAuthenticationSubject$ = new Subject<boolean>();
  waitUntilAuthentication$ =
    this._waitUntilAuthenticationSubject$.asObservable();

  async init(authConfig: AuthConfig) {
    this.oauthService.configure({
      ...this._authConfig,
      ...authConfig,
    });

    this.oauthService.timeoutFactor = 0.7;

    this.oauthService.setupAutomaticSilentRefresh();

    this.oauthService.events.subscribe({
      next: (event: OAuthEvent) => {
        switch (event.type) {
          case 'token_refresh_error':
            console.error('Session Timeout: Token Refresh Error', event);

            break;

          default:
            break;
        }
      },
    });

    try {
      const isLoggedIn =
        await this.oauthService.loadDiscoveryDocumentAndLogin();

      if (isLoggedIn) {
        const profile = this.getProfile();
        // TODO: API call to fetch more data about user?
        // TODO: most likely profile is organization, not the user
        this.profileStore.setProfile({
          name: profile['name'],
          email: profile['email'],
          organization: '',
        });

        this.isDoneLoading.set(true);
        this.isAuthenticationSuccessful.set(true);
        this._waitUntilAuthenticationSubject$.next(true);
      } else {
        // Pass in the original URL as additional state to the identity provider.  This information will be
        // returned once the user has been authenticated and will be used to redirect the user to the
        // originally requested URL.
        // DO NOT URL encode the state value as that happens automatically by the OAuthService.  Just convert
        // to base64.
        const encodedState = btoa(
          JSON.stringify({ originalURL: window.location.href })
        );

        this.oauthService.initCodeFlow(encodedState);
      }
    } catch (error) {
      this.isDoneLoading.set(true);
      this.isAuthenticationSuccessful.set(false);
      this._waitUntilAuthenticationSubject$.next(false);
    }
  }

  getProfile() {
    // get data from jwt token
    return this.oauthService.getIdentityClaims();
  }

  isLoggedIn() {
    return (
      this.oauthService.hasValidAccessToken() &&
      this.oauthService.hasValidIdToken()
    );
  }

  logout() {
    this.oauthService.logOut();
  }
}
