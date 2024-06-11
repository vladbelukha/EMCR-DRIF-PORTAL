import { Routes } from '@angular/router';
import { AuthenticationGuard } from './core/guards/authentication.guard';
import { EOIApplicationComponent } from './eoi-application/eoi-application.component';
import { LandingPageComponent } from './landing-page/landing-page.component';
import { SuccessPageComponent } from './success-page/success-page.component';

export const routes: Routes = [
  { path: '', component: LandingPageComponent },
  { path: 'dashboard', component: LandingPageComponent },
  {
    path: 'eoi-application',
    component: EOIApplicationComponent,
    canActivate: [AuthenticationGuard],
  },
  { path: 'success', component: SuccessPageComponent },
];
