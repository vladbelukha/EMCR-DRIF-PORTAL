import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { AuthService } from '../../core/auth/auth.service';
import { ProfileStore } from '../../store/profile.store';

@Component({
  selector: 'drr-header',
  standalone: true,
  imports: [
    CommonModule,
    MatDividerModule,
    MatToolbarModule,
    MatMenuModule,
    TranslocoModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class HeaderComponent {
  router = inject(Router);
  profileStore = inject(ProfileStore);
  authService = inject(AuthService);

  homeClick() {
    this.router.navigate(['/dashboard']);
  }

  signOut() {
    this.authService.logout();
  }
}
