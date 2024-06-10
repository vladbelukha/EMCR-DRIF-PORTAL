import { Routes } from '@angular/router';
import { AuthenticationGuard } from './core/guards/authentication.guard';
import { EOIApplicationComponent } from './eoi-application/eoi-application.component';
import { SuccessPageComponent } from './success-page/success-page.component';

export const routes: Routes = [
  { path: '', redirectTo: '/eoi-application', pathMatch: 'full' },
  // { path: 'register', component: RegisterPageComponent },
  {
    path: 'eoi-application',
    component: EOIApplicationComponent,
    canActivate: [AuthenticationGuard],
  },
  { path: 'success', component: SuccessPageComponent },
];
