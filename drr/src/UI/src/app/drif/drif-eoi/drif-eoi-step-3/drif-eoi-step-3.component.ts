import { BreakpointObserver } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { Component, Input, inject } from '@angular/core';
import {
  FormArray,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { distinctUntilChanged } from 'rxjs';
import { FundingType } from '../../../../model';

import { UntilDestroy } from '@ngneat/until-destroy';
import { DrrCurrencyInputComponent } from '../../../shared/controls/drr-currency-input/drr-currency-input.component';
import { DrrInputComponent } from '../../../shared/controls/drr-input/drr-input.component';
import { DrrRadioButtonComponent } from '../../../shared/controls/drr-radio-button/drr-radio-button.component';
import { DrrSelectComponent } from '../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import { DrrFundingListComponent } from '../../drr-funding-list/drr-funding-list.component';
import {
  FundingInformationForm,
  FundingInformationItemForm,
} from '../drif-eoi-form';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drif-eoi-step-3',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatSelectModule,
    TranslocoModule,
    DrrTextareaComponent,
    DrrInputComponent,
    DrrSelectComponent,
    DrrCurrencyInputComponent,
    DrrFundingListComponent,
    DrrRadioButtonComponent,
  ],
  templateUrl: './drif-eoi-step-3.component.html',
  styleUrl: './drif-eoi-step-3.component.scss',
})
export class DrifEoiStep3Component {
  breakpointObserver = inject(BreakpointObserver);
  isMobile = false;

  @Input()
  fundingInformationForm!: IFormGroup<FundingInformationForm>;

  fundingTypeOptions = Object.values(FundingType);

  formBuilder = inject(RxFormBuilder);

  ngOnInit() {
    this.fundingInformationForm
      .get('otherFunding')!
      .valueChanges.pipe(distinctUntilChanged())
      .subscribe(() => {
        this.calculateRemainingAmount();
      });

    this.fundingInformationForm
      .get('fundingRequest')!
      .valueChanges.pipe(distinctUntilChanged())
      .subscribe(() => {
        this.calculateRemainingAmount();
      });

    this.fundingInformationForm
      .get('estimatedTotal')!
      .valueChanges.pipe(distinctUntilChanged())
      .subscribe(() => {
        this.calculateRemainingAmount();
      });

    this.fundingInformationForm
      .get('haveOtherFunding')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        if (value) {
          this.getFormArray('otherFunding').enable();
          if (this.getFormArray('otherFunding').length === 0) {
            this.getFormArray('otherFunding').push(
              this.formBuilder.formGroup(FundingInformationItemForm)
            );
          }
        } else {
          this.getFormArray('otherFunding').clear();
          this.getFormArray('otherFunding').disable();
        }
      });

    this.breakpointObserver
      .observe('(min-width: 768px)')
      .subscribe(({ matches }) => {
        this.isMobile = !matches;
      });
  }

  calculateRemainingAmount() {
    const estimatedTotal =
      this.fundingInformationForm.get('estimatedTotal')?.value ?? 0;

    let otherFundingSum = this.getFormArray('otherFunding').controls.reduce(
      (total, funding) => total + Number(funding.value.amount),
      0
    );
    // check if number
    if (isNaN(otherFundingSum)) {
      otherFundingSum = 0;
    }

    const fundingRequest =
      this.fundingInformationForm.get('fundingRequest')?.value ?? 0;

    let remainingAmount = estimatedTotal - otherFundingSum - fundingRequest;

    this.fundingInformationForm.patchValue({ remainingAmount });

    const intendToSecureFunding = this.fundingInformationForm.get(
      'intendToSecureFunding'
    );

    if (remainingAmount > 0) {
      intendToSecureFunding?.addValidators(Validators.required);
    } else {
      intendToSecureFunding?.clearValidators();
      intendToSecureFunding?.reset();
    }

    intendToSecureFunding?.updateValueAndValidity();
  }

  getFormArray(formArrayName: string) {
    return this.fundingInformationForm.get(formArrayName) as FormArray;
  }

  addOtherFunding() {
    this.getFormArray('otherFunding').push(
      this.formBuilder.formGroup(FundingInformationItemForm)
    );
  }

  removeOtherSource(index: number) {
    this.getFormArray('otherFunding').removeAt(index);
  }

  showRemainingAmount() {
    const fundingRequestValue =
      this.fundingInformationForm.get('fundingRequest')?.value;
    const estimatedTotalValue =
      this.fundingInformationForm.get('estimatedTotal')?.value;

    if (fundingRequestValue === 0 || estimatedTotalValue === 0) {
      return true;
    }

    return !!fundingRequestValue && !!estimatedTotalValue;
  }

  getRemainingAmount() {
    return this.fundingInformationForm.get('remainingAmount')?.value;
  }

  getRemainingAmountAbs() {
    return Math.abs(this.getRemainingAmount());
  }

  hasOtherGrants(selectValue: FundingType[]) {
    return selectValue?.includes(FundingType.OtherGrants);
  }
}
