import { Routes } from '@angular/router';
import { RegisterPageComponent } from './register-page/register-page.component';
import { EOIApplicationComponent } from './eoi-application/eoi-application.component';

export const routes: Routes = [
    { path: '', redirectTo: '/register', pathMatch: 'full' },
    { path: 'register', component: RegisterPageComponent },
    { path: 'eoi-application', component: EOIApplicationComponent }
];
