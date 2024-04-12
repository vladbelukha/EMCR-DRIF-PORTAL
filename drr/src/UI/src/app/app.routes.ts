import { Routes } from '@angular/router';
import { RegisterPageComponent } from './register-page/register-page.component';
import { StartApplicationComponent } from './start-application/start-application.component';

export const routes: Routes = [
    { path: '', redirectTo: '/register', pathMatch: 'full' },
    { path: 'register', component: RegisterPageComponent },
    { path: 'start-application', component: StartApplicationComponent }
];
