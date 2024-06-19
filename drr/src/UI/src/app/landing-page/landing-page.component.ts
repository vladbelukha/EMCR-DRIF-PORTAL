import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { ConfigurationStore } from '../store/configuration.store';

@Component({
  selector: 'drr-landing-page',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatButtonModule, TranslocoModule],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.scss',
})
export class LandingPageComponent {
  router = inject(Router);
  configurationStore = inject(ConfigurationStore);

  loginClick() {
    // TODO: maybe trigger login before navigating? but guard can trigger it as well more universally
    this.router.navigate(['/dashboard']);
  }
}
