import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';

@Component({
  selector: 'drr-register-page',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './register-page.component.html',
  styleUrl: './register-page.component.scss',
})
export class RegisterPageComponent {
  router = inject(Router);

  startApplication() {
    this.router.navigate(['/eoi-application']);
  }
}
