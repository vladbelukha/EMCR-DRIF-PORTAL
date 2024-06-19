import { Routes } from '@angular/router';
import { AuthenticationGuard } from './core/guards/authentication.guard';
import { DashboardComponent } from './dashboard/dashboard.component';
import { EOIApplicationComponent } from './eoi-application/eoi-application.component';
import { LandingPageComponent } from './landing-page/landing-page.component';
import { ProfileComponent } from './profile/profile.component';
import { SuccessPageComponent } from './success-page/success-page.component';

export const routes: Routes = [
  { path: '', component: LandingPageComponent },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'eoi-application',
    component: EOIApplicationComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'eoi-application/:id',
    component: EOIApplicationComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'success',
    component: SuccessPageComponent,
    canActivate: [AuthenticationGuard],
  },
];
