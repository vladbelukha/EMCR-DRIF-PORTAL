import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import {
  MatStepperModule,
  StepperOrientation,
} from '@angular/material/stepper';
import { TranslocoModule } from '@ngneat/transloco';

@Component({
  selector: 'drr-drif-interim-report-create',
  standalone: true,
  imports: [
    CommonModule,
    MatStepperModule,
    MatIconModule,
    MatButtonModule,
    TranslocoModule,
  ],
  templateUrl: './drif-interim-report-create.component.html',
  styleUrl: './drif-interim-report-create.component.scss',
})
export class DrifInterimReportCreateComponent {
  stepperOrientation: StepperOrientation = 'horizontal';

  stepperSelectionChange(event: any) {}

  save() {}

  goBack() {}
}
