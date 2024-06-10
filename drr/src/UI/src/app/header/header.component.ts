import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, Component, inject } from '@angular/core';
import { MatDividerModule } from '@angular/material/divider';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { TranslocoModule } from '@ngneat/transloco';
import { AuthService } from '../core/auth/auth.service';
import { ProfileStore } from '../store/profile.store';

@Component({
  selector: 'drr-header',
  standalone: true,
  imports: [
    CommonModule,
    MatDividerModule,
    MatToolbarModule,
    MatMenuModule,
    TranslocoModule,
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class HeaderComponent {
  profileStore = inject(ProfileStore);
  authService = inject(AuthService);

  homeClick() {}

  signOut() {
    this.authService.logout();
  }

  isAuthenticated() {
    return this.profileStore.loggedIn();
  }
}
