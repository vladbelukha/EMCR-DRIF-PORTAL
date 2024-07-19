import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { ActivatedRoute, Router } from '@angular/router';
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
  route = inject(ActivatedRoute);

  id?: string;
  fundingStream?: string;

  ngOnInit() {
    // get ID from route and funding stream from query params
    this.id = this.route.snapshot.params['id'];
    this.fundingStream = this.route.snapshot.queryParams['fundingStream'];
  }

  goBack() {
    this.router.navigate(['/dashboard']);
  }

  continue() {
    // make a call
    this.router.navigate(['/drif-fp', 'DRIF-FP-1111']);
  }
}
