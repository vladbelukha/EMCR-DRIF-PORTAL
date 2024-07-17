import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';

@Component({
  selector: 'drr-drif-fp-instructions',
  standalone: true,
  imports: [CommonModule, MatButtonModule, TranslocoModule],
  templateUrl: './drif-fp-instructions.component.html',
  styleUrl: './drif-fp-instructions.component.scss',
})
export class DrifFpInstructionsComponent {
  router = inject(Router);

  goBack() {
    this.router.navigate(['/dashboard']);
  }

  continue() {
    // make a call
    this.router.navigate(['/drif-fp']);
  }
}
