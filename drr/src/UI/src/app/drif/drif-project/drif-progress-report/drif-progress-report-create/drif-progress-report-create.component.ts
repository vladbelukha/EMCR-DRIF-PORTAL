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
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { YesNoOption } from '../../../../../model';
import { DrrDatepickerComponent } from '../../../../shared/controls/drr-datepicker/drr-datepicker.component';
import { DrrInputComponent } from '../../../../shared/controls/drr-input/drr-input.component';
import {
  DrrRadioButtonComponent,
  RadioOption,
} from '../../../../shared/controls/drr-radio-button/drr-radio-button.component';
import {
  DrrSelectComponent,
  DrrSelectOption,
} from '../../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../../shared/controls/drr-textarea/drr-textarea.component';
import { ProgressReportForm } from '../drif-progress-report-form';

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
    DrrInputComponent,
    DrrSelectComponent,
    DrrRadioButtonComponent,
    DrrTextareaComponent,
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
  yesNoNaOptions = Object.values(YesNoOption).map((value) => ({
    label: value, // TODO: translate
    value,
  }));

  yesNoOptions: DrrSelectOption[] = [
    {
      label: 'Yes',
      value: 'yes',
    },
    {
      label: 'No',
      value: 'no',
    },
  ];

  stepperOrientation: StepperOrientation = 'horizontal';

  progressReportForm = this.formBuilder.formGroup(
    ProgressReportForm
  ) as IFormGroup<ProgressReportForm>;

  stepperSelectionChange(event: any) {}

  save() {}

  goBack() {}
}
