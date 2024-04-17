import { Routes } from '@angular/router';
import { RegisterPageComponent } from './register-page/register-page.component';
import { EOIApplicationComponent } from './eoi-application/eoi-application.component';
import { SuccessPageComponent } from './success-page/success-page.component';

export const routes: Routes = [
  { path: '', redirectTo: '/register', pathMatch: 'full' },
  { path: 'register', component: RegisterPageComponent },
  { path: 'eoi-application', component: EOIApplicationComponent },
  { path: 'success', component: SuccessPageComponent },
];
