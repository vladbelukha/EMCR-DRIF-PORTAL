import {
  provideHttpClient,
  withFetch,
  withInterceptors,
} from '@angular/common/http';
import {
  APP_INITIALIZER,
  ApplicationConfig,
  importProvidersFrom,
  isDevMode,
} from '@angular/core';
import { provideLuxonDateAdapter } from '@angular/material-luxon-adapter';
import { MAT_DATE_FORMATS, MatDateFormats } from '@angular/material/core';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS } from '@angular/material/form-field';
import { provideClientHydration } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { provideHotToastConfig } from '@ngneat/hot-toast';
import { provideTransloco } from '@ngneat/transloco';
import { provideOAuthClient } from 'angular-oauth2-oidc';
import { provideNgxMask } from 'ngx-mask';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ConfigurationService } from '../api/configuration/configuration.service';
import { DrifapplicationService } from '../api/drifapplication/drifapplication.service';
import { LoadingInterceptor } from '../interceptors/loading.interceptor';
import { routes } from './app.routes';
import { AuthService } from './core/auth/auth.service';
import { TokenInterceptor } from './core/interceptors/token.interceptor';
import { TranslocoHttpLoader } from './transloco-loader';

export const DRR_DATE_FORMATS: MatDateFormats = {
  parse: {
    dateInput: 'yyyy-MM-dd',
  },
  display: {
    dateInput: 'yyyy-MM-dd',
    monthYearLabel: 'yyyy',
    dateA11yLabel: 'yyyy-MM-dd',
    monthYearA11yLabel: 'yyyy',
  },
};

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideClientHydration(),
    provideHttpClient(
      withFetch(),
      withInterceptors([LoadingInterceptor, TokenInterceptor])
    ),
    provideOAuthClient(),
    provideAnimations(),
    provideLuxonDateAdapter(),
    {
      provide: MAT_DATE_FORMATS,
      useValue: DRR_DATE_FORMATS,
    },
    provideHotToastConfig({
      autoClose: false,
      dismissible: true,
    }),
    DrifapplicationService,
    provideHttpClient(),
    provideTransloco({
      config: {
        availableLangs: ['en'],
        defaultLang: 'en',
        // Remove this option if your application doesn't support changing language in runtime.
        reRenderOnLangChange: true,
        prodMode: !isDevMode(),
      },
      loader: TranslocoHttpLoader,
    }),
    {
      provide: APP_INITIALIZER,
      deps: [ConfigurationService, AuthService],
      useFactory:
        (
          configurationService: ConfigurationService,
          authService: AuthService
        ) =>
        async () => {
          // const url = '/config';
          // const config = await firstValueFrom(http.get<AuthConfig>(url));
          const config = {
            'confidential-port': 0,
            'auth-server-url': 'https://dev.loginproxy.gov.bc.ca/auth',
            realm: 'standard',
            'ssl-required': 'external',
            resource: '',
            credentials: {
              secret: '',
            },
            requireHttps: false,
          };

          await authService.init(config);
          console.log('APP_INITIALIZER');

          // TODO: fetch observable instead?
          return Promise.resolve(true);
        },
      multi: true,
    },
    importProvidersFrom(NgxSpinnerModule),
    {
      provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
      useValue: {
        hideRequiredMarker: false,
        floatLabel: 'always',
      },
    },
    provideNgxMask(),
  ],
};
