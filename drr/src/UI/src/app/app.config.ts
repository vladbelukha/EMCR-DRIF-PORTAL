import {
  ApplicationConfig,
  importProvidersFrom,
  isDevMode,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideLuxonDateAdapter } from '@angular/material-luxon-adapter';
import { DrifapplicationService } from '../api/drifapplication/drifapplication.service';
import {
  provideHttpClient,
  withFetch,
  withInterceptors,
} from '@angular/common/http';
import { TranslocoHttpLoader } from './transloco-loader';
import { provideTransloco } from '@ngneat/transloco';
import { provideHotToastConfig } from '@ngneat/hot-toast';
import {
  MAT_DATE_FORMATS,
  MAT_NATIVE_DATE_FORMATS,
  MatDateFormats,
} from '@angular/material/core';
import { NgxSpinnerModule } from 'ngx-spinner';
import { LoadingInterceptor } from '../interceptors/loading.interceptor';

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
    provideHttpClient(withFetch(), withInterceptors([LoadingInterceptor])),
    provideAnimations(),
    provideLuxonDateAdapter(),
    {
      provide: MAT_DATE_FORMATS,
      useValue: DRR_DATE_FORMATS,
    },
    provideHotToastConfig(),
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
    importProvidersFrom(NgxSpinnerModule),
  ],
};
