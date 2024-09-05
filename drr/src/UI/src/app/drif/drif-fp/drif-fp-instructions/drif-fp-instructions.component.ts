import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { DrifapplicationService } from '../../../../api/drifapplication/drifapplication.service';
import { ScreenerQuestions } from '../../../../model';

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
  appService = inject(DrifapplicationService);

  eoiId?: string;
  fundingStream?: string;
  screenerQuestions?: ScreenerQuestions;

  ngOnInit() {
    this.eoiId = this.route.snapshot.params['eoiId'];
    this.fundingStream = this.route.snapshot.params['fundingStream'];
    this.screenerQuestions = this.route.snapshot.queryParams;
  }

  goBack() {
    this.router.navigate(['/dashboard']);
  }

  continue() {
    this.appService
      .dRIFApplicationCreateFPFromEOI(
        {
          ...this.screenerQuestions,
        },
        {
          eoiId: this.eoiId,
        }
      )
      .subscribe((res) => {
        this.router.navigate(['/drif-fp', res.id]);
      });
  }
}
