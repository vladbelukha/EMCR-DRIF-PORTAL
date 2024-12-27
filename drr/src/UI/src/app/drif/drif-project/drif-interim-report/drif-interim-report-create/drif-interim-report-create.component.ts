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
import { DrrDatepickerComponent } from '../../../../shared/controls/drr-datepicker/drr-datepicker.component';
import {
  DrrSelectComponent,
  DrrSelectOption,
} from '../../../../shared/controls/drr-select/drr-select.component';

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

  stepperOrientation: StepperOrientation = 'horizontal';

  // TODO: replace with a form
  form = this.formBuilder.group({
    reportType: [null],
    reportDate: [new Date()],
    reportDueDate: [new Date()],
  });

  reportTypeOptions?: DrrSelectOption[] = [
    { value: 'interim', label: 'Interim Report' },
    { value: 'offCycle', label: 'Request Off-cycle payment' },
    { value: 'final', label: 'Final' },
    { value: 'skip', label: 'Request to skip report' },
  ];

  ngOnInit() {
    this.form.get('reportType')?.valueChanges.subscribe((value) => {
      console.log('reportTypeControl value changed', value);
    });
  }

  stepperSelectionChange(event: any) {}

  save() {}

  goBack() {}
}
