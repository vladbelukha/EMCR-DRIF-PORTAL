import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormArray } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import {
  MatStepperModule,
  StepperOrientation,
} from '@angular/material/stepper';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { DrrCurrencyInputComponent } from '../../../../shared/controls/drr-currency-input/drr-currency-input.component';
import { DrrDatepickerComponent } from '../../../../shared/controls/drr-datepicker/drr-datepicker.component';
import { DrrInputComponent } from '../../../../shared/controls/drr-input/drr-input.component';
import { DrrRadioButtonComponent } from '../../../../shared/controls/drr-radio-button/drr-radio-button.component';
import { DrrSelectComponent } from '../../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../../shared/controls/drr-textarea/drr-textarea.component';
import { ForecastForm, YearForecastForm } from '../drif-forecast-form';

@Component({
  selector: 'drr-drif-forecast-create',
  standalone: true,
  imports: [
    CommonModule,
    MatStepperModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    MatCardModule,
    TranslocoModule,
    DrrDatepickerComponent,
    DrrInputComponent,
    DrrSelectComponent,
    DrrRadioButtonComponent,
    DrrTextareaComponent,
    DrrCurrencyInputComponent,
  ],
  templateUrl: './drif-forecast-create.component.html',
  styleUrl: './drif-forecast-create.component.scss',
  providers: [RxFormBuilder],
})
export class DrifForecastCreateComponent {
  formBuilder = inject(RxFormBuilder);

  stepperOrientation: StepperOrientation = 'horizontal';

  forecastForm = this.formBuilder.formGroup(
    ForecastForm
  ) as IFormGroup<ForecastForm>;

  ngOnInit() {
    // TODO: temp add init values
    this.getYearForecastFormArray().controls.push(
      this.formBuilder.formGroup(YearForecastForm, {
        fiscalYear: 2021,
        originalForecast: 1000,
        projectedExpenditure: 900,
        paidClaimsAmount: 800,
        outstandingClaimsAmount: 100,
        remainingClaimsAmount: 100,
      })
    );
    this.getYearForecastFormArray().controls.push(
      this.formBuilder.formGroup(YearForecastForm, {
        fiscalYear: 2022,
        originalForecast: 2000,
        projectedExpenditure: 1900,
        paidClaimsAmount: 1800,
        outstandingClaimsAmount: 200,
        remainingClaimsAmount: 200,
      })
    );
    this.getYearForecastFormArray().disable();
  }

  getYearForecastFormArray() {
    return this.forecastForm.get('yearForecasts') as FormArray;
  }

  stepperSelectionChange(event: any) {}

  goBack() {}

  save() {}
}
