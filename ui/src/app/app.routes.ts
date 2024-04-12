import { Routes } from '@angular/router';
import { RegisterPageComponent } from './register-page/register-page.component';

export const routes: Routes = [
    { path: '', redirectTo: '/register', pathMatch: 'full' },
    { path: 'register', component: RegisterPageComponent },
];
