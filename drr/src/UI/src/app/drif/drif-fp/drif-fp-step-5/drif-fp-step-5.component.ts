import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import {
  FormArray,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectChange } from '@angular/material/select';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { FundingType } from '../../../../model';
import { DrrCurrencyInputComponent } from '../../../shared/controls/drr-currency-input/drr-currency-input.component';
import { DrrInputComponent } from '../../../shared/controls/drr-input/drr-input.component';
import { DrrSelectComponent } from '../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import { FundingInformationItemForm } from '../../drif-eoi/drif-eoi-form';
import { BudgetForm, YearOverYearFundingForm } from '../drif-fp-form';

@Component({
  selector: 'drif-fp-step-5',
  standalone: true,
  imports: [
    CommonModule,
    MatInputModule,
    DrrInputComponent,
    DrrCurrencyInputComponent,
    DrrSelectComponent,
    DrrTextareaComponent,
    TranslocoModule,
    FormsModule,
    ReactiveFormsModule,
    MatIconModule,
    MatButtonModule,
  ],
  templateUrl: './drif-fp-step-5.component.html',
  styleUrl: './drif-fp-step-5.component.scss',
})
export class DrifFpStep5Component {
  formBuilder = inject(RxFormBuilder);

  @Input()
  budgetForm!: IFormGroup<BudgetForm>;

  isMobile = false;

  fiscalYearsOptions = ['2023/2024'];
  fundingTypeOptions = Object.values(FundingType);

  getFormArray(formArrayName: string) {
    return this.budgetForm.get(formArrayName) as FormArray;
  }

  getArrayFormControl(
    formArrayName: string,
    controlName: string,
    index: number
  ) {
    return this.getFormArray(formArrayName)?.at(index).get(controlName);
  }

  addYear() {
    this.getFormArray('yearOverYearFunding').push(
      this.formBuilder.formGroup(YearOverYearFundingForm)
    );
  }

  removeYear(index: number) {
    this.getFormArray('years').removeAt(index);
  }

  hasOtherGrants(selectValue: FundingType[]) {
    return selectValue?.includes(FundingType.OtherGrants);
  }

  setFundingTypeDesctiption(event: MatSelectChange, index: number) {
    const descriptionControl = this.getArrayFormControl(
      'otherDescription',
      'otherFunding',
      index
    );

    // check if value contains FundingType.OtherGrants
    if (this.hasOtherGrants(event.value)) {
      descriptionControl?.addValidators(Validators.required);
    } else {
      descriptionControl?.clearValidators();
    }

    descriptionControl?.reset();
    descriptionControl?.updateValueAndValidity();
  }

  removeOtherSource(index: number) {
    this.getFormArray('otherFunding').removeAt(index);
  }

  addOtherFunding() {
    this.getFormArray('otherFunding').push(
      this.formBuilder.formGroup(FundingInformationItemForm)
    );
  }
}
