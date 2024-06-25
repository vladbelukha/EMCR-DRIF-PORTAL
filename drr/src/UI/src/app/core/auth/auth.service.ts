import { Injectable, inject, signal } from '@angular/core';
import { AuthConfig, OAuthEvent, OAuthService } from 'angular-oauth2-oidc';
import { Subject, firstValueFrom } from 'rxjs';
import { ProfileService } from '../../../api/profile/profile.service';
import { ConfigurationStore } from '../../store/configuration.store';
import { ProfileStore } from '../../store/profile.store';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  oauthService = inject(OAuthService);
  profileStore = inject(ProfileStore);
  configurationStore = inject(ConfigurationStore);
  profileService = inject(ProfileService);

  isAuthenticationSuccessful = signal(false);

  private _waitUntilAuthenticationSubject$ = new Subject<boolean>();
  waitUntilAuthentication$ =
    this._waitUntilAuthenticationSubject$.asObservable();

  async setProfile() {
    if (this.isLoggedIn()) {
      const profile = this.getProfile();
      const profileDetails = await firstValueFrom(
        this.profileService.profileProfileDetails()
      );

      this.profileStore.setProfile({
        fullName: profile['name'],
        firstName: profileDetails.firstName,
        lastName: profileDetails.lastName,
        title: profileDetails.title,
        department: profileDetails.department,
        phone: profileDetails.phone,
        email: profile['email'],
        organization: profile['bceid_business_name'],
        loggedIn: true,
      });

      return true;
    }

    return false;
  }

  async login(customConfiuration?: AuthConfig) {
    const authConfig: AuthConfig = {
      responseType: 'code',
      strictDiscoveryDocumentValidation: false,
      showDebugInformation: false,
      requireHttps: true,
      redirectUri: window.location.origin + '/dashboard',
      openUri: (uri: string) => {
        const url = new URL(uri);

        // url.searchParams.delete('id_token_hint');
        // reconstruct url
        const reconstructUri = `${url.origin}${
          url.pathname
        }?${url.searchParams.toString()}`;

        location.href = reconstructUri;
      },
      postLogoutRedirectUri: window.location.origin, // TODO: maybe it supposed to be landing page?
    };

    if (this.isLoggedIn()) {
      this.isAuthenticationSuccessful.set(true);
      this._waitUntilAuthenticationSubject$.next(true);

      this.setProfile();

      return true;
    }

    if (!this.configurationStore.isConfigurationLoaded()) {
      console.error('Configuration is not loaded yet');
      return false;
    }

    const configuration = this.configurationStore.oidc!();

    this.oauthService.configure({
      ...authConfig,
      ...configuration,
      ...customConfiuration,
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
        this.setProfile();

        this.isAuthenticationSuccessful.set(true);
        this._waitUntilAuthenticationSubject$.next(true);
        return true;
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
        return false;
      }
    } catch (error) {
      this.isAuthenticationSuccessful.set(false);
      this._waitUntilAuthenticationSubject$.next(false);

      return false;
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
