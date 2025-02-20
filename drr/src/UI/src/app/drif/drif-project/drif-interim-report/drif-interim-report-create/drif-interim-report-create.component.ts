import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import {
  MatStepperModule,
  StepperOrientation,
} from '@angular/material/stepper';
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { PeriodType } from '../../../../../model';
import { DrrDatepickerComponent } from '../../../../shared/controls/drr-datepicker/drr-datepicker.component';
import {
  DrrSelectComponent,
  DrrSelectOption,
} from '../../../../shared/controls/drr-select/drr-select.component';
import { InterimReportForm } from '../drif-interim-report-form';

@Component({
  selector: 'drr-drif-interim-report-create',
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
  ],
  templateUrl: './drif-interim-report-create.component.html',
  styleUrl: './drif-interim-report-create.component.scss',
  providers: [RxFormBuilder],
})
export class DrifInterimReportCreateComponent {
  formBuilder = inject(RxFormBuilder);
  translocoService = inject(TranslocoService);

  stepperOrientation: StepperOrientation = 'horizontal';

  interimReportForm = this.formBuilder.formGroup(
    InterimReportForm,
  ) as IFormGroup<InterimReportForm>;

  periodTypeOptions?: DrrSelectOption[] = Object.keys(PeriodType).map(
    (value) => {
      return {
        value,
        label: this.translocoService.translate(`periodType.${value}`),
      };
    },
  );

  ngOnInit() {
    this.interimReportForm.get('type')?.valueChanges.subscribe((value) => {
      console.log('reportTypeControl value changed', value);
    });
  }

  stepperSelectionChange(event: any) {}

  save() {}

  goBack() {}
}
