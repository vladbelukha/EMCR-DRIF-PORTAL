import { Routes } from '@angular/router';
import { EOIApplicationComponent } from './eoi-application/eoi-application.component';
import { SuccessPageComponent } from './shared/components/success-page/success-page.component';

export const routes: Routes = [
  { path: '', redirectTo: '/eoi-application', pathMatch: 'full' },
  // { path: 'register', component: RegisterPageComponent },
  { path: 'eoi-application', component: EOIApplicationComponent },
  { path: 'success', component: SuccessPageComponent },
];
