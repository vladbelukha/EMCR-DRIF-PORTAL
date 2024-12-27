import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import {
  MatStepperModule,
  StepperOrientation,
} from '@angular/material/stepper';
import { TranslocoModule } from '@ngneat/transloco';
import { RxFormBuilder } from '@rxweb/reactive-form-validators';
import { DrrDatepickerComponent } from '../../../shared/controls/drr-datepicker/drr-datepicker.component';
import {
  DrrRadioButtonComponent,
  RadioOption,
} from '../../../shared/controls/drr-radio-button/drr-radio-button.component';
import { DrrSelectComponent } from '../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from "../../../shared/controls/drr-textarea/drr-textarea.component";

@Component({
  selector: 'drr-drif-progress-report-create',
  standalone: true,
  imports: [
    CommonModule,
    MatStepperModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    TranslocoModule,
    DrrDatepickerComponent,
    DrrSelectComponent,
    DrrRadioButtonComponent,
    DrrTextareaComponent
],
  templateUrl: './drif-progress-report-create.component.html',
  styleUrl: './drif-progress-report-create.component.scss',
  providers: [RxFormBuilder],
})
export class DrifProgressReportCreateComponent {
  formBuilder = inject(RxFormBuilder);

  progressReportOptions: RadioOption[] = [
    {
      label: 'Not Started',
      value: 'notStarted',
    },
    {
      label: 'In Progress',
      value: 'inProgress',
    },
    {
      label: 'Completed',
      value: 'completed',
    },
    {
      label: 'Not Applicable',
      value: 'notApplicable',
    },
  ];

  stepperOrientation: StepperOrientation = 'horizontal';

  form = this.formBuilder.group({
    projectProgress: [null],
    projectProgressComment: [null],
    projectProgressDate: [null],
    firstNationEngagementProgress: [null],
    firstNationEngagementProgressComment: [null],
    firstNationEngagementProgressDate: [null],
    designProgress: [null],
    designProgressComment: [null],
    designProgressDate: [null],
    constructionTenderProgress: [null],
    constructionTenderProgressComment: [null],
    constructionTenderProgressDate: [null],
    constractionContractProgress: [null],
    constractionContractProgressComment: [null],
    constractionContractProgressDate: [null],
    permitToConstructProgress: [null],
    permitToConstructProgressComment: [null],
    permitToConstructProgressDate: [null],
    constructionProgress: [null],
    constructionProgressComment: [null],
    constructionProgressDate: [null],
  });

  stepperSelectionChange(event: any) {}

  save() {}

  goBack() {}
}
