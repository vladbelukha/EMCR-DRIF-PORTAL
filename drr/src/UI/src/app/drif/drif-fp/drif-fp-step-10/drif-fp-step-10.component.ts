import { CommonModule, CurrencyPipe } from '@angular/common';
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
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { distinctUntilChanged } from 'rxjs';
import { FundingType, YesNoOption } from '../../../../model';
import { DrrChipAutocompleteComponent } from '../../../shared/controls/drr-chip-autocomplete/drr-chip-autocomplete.component';
import { DrrCurrencyInputComponent } from '../../../shared/controls/drr-currency-input/drr-currency-input.component';
import { DrrInputComponent } from '../../../shared/controls/drr-input/drr-input.component';
import { DrrRadioButtonComponent } from '../../../shared/controls/drr-radio-button/drr-radio-button.component';
import { DrrSelectComponent } from '../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import { OptionsStore } from '../../../store/options.store';
import { FundingInformationItemForm } from '../../drif-eoi/drif-eoi-form';
import { DrrFundingListComponent } from '../../drr-funding-list/drr-funding-list.component';
import { BudgetForm, YearOverYearFundingForm } from '../drif-fp-form';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drif-fp-step-10',
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
    DrrFundingListComponent,
    DrrRadioButtonComponent,
    DrrChipAutocompleteComponent,
  ],
  providers: [CurrencyPipe],
  templateUrl: './drif-fp-step-10.component.html',
  styleUrl: './drif-fp-step-10.component.scss',
})
export class DrifFpStep10Component {
  optionsStore = inject(OptionsStore);
  formBuilder = inject(RxFormBuilder);
  translocoService = inject(TranslocoService);

  @Input()
  budgetForm!: IFormGroup<BudgetForm>;

  isMobile = false;

  fiscalYearsOptions =
    this.optionsStore.fiscalYears?.()?.map((value) => ({
      value,
      label: value,
    })) ?? [];
  fundingTypeOptions = Object.values(FundingType).map((value) => ({
    value,
    label: this.translocoService.translate(value),
  }));
  previousResponseOptions = [
    { value: YesNoOption.Yes, label: 'Yes' },
    { value: YesNoOption.NotApplicable, label: 'Yes, but costs unknown' },
    { value: YesNoOption.No, label: 'No' },
  ];
  costConsiderationsOptions = this.optionsStore.costConsiderations?.() ?? [];

  ngOnInit() {
    const currentYear = new Date().getFullYear();
    const currentMonth = new Date().getMonth();
    const startFiscalYear = currentMonth >= 6 ? currentYear : currentYear - 1; // Assuming fiscal year starts in July

    // this.fiscalYearsOptions = Array.from({ length: 10 }, (_, i) => {
    //   const startYear = startFiscalYear + i;
    //   const endYear = startYear + 1;
    //   return `${startYear}/${endYear}`;
    // });

    this.budgetForm
      .get('yearOverYearFunding')!
      .valueChanges.pipe(distinctUntilChanged())
      .subscribe((years: YearOverYearFundingForm[]) => {
        const total = years.reduce((acc, year) => acc + Number(year.amount), 0);
        this.budgetForm.get('totalDrifFundingRequest')?.setValue(total);

        if (total !== this.budgetForm.get('fundingRequest')?.value) {
          this.budgetForm
            .get('discrepancyComment')
            ?.setValidators(Validators.required);
        } else {
          this.budgetForm.get('discrepancyComment')?.clearValidators();
        }

        this.budgetForm.get('discrepancyComment')?.updateValueAndValidity();
      });

    this.budgetForm
      .get('otherFunding')!
      .valueChanges.pipe(distinctUntilChanged())
      .subscribe(() => {
        this.calculateRemainingAmount();
      });

    this.budgetForm
      .get('fundingRequest')!
      .valueChanges.pipe(distinctUntilChanged())
      .subscribe(() => {
        this.calculateRemainingAmount();
      });

    this.budgetForm
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

    this.budgetForm
      .get('costConsiderationsApplied')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        if (value) {
          this.budgetForm
            .get('costConsiderations')
            ?.setValidators(Validators.required);
          this.budgetForm
            .get('costConsiderationsComments')
            ?.setValidators(Validators.required);
        } else {
          this.budgetForm.get('costConsiderations')?.clearValidators();
          this.budgetForm.get('costConsiderationsComments')?.clearValidators();
        }

        this.budgetForm.get('costConsiderations')?.updateValueAndValidity();
        this.budgetForm
          .get('costConsiderationsComments')
          ?.updateValueAndValidity();
      });
  }

  showDiscrepancyComment() {
    return (
      this.budgetForm.get('totalDrifFundingRequest')?.value !==
      this.budgetForm.get('fundingRequest')?.value
    );
  }

  calculateRemainingAmount() {
    // how much I'm covering with other funding
    let otherFundingSum = this.getFormArray('otherFunding').controls.reduce(
      (total, funding) => total + Number(funding.value.amount),
      0
    );
    // check if number
    if (isNaN(otherFundingSum)) {
      otherFundingSum = 0;
    }

    // how much will the project cost, but not how much I'm asking for
    const fundingRequest = this.budgetForm.get('fundingRequest')?.value ?? 0;
    // how much I'm asking for
    const totalDrifFundingRequest =
      this.budgetForm.get('totalDrifFundingRequest')?.value ?? 0;

    // how much is left to cover and I need to explain how I'm going to cover it
    let remainingAmount =
      fundingRequest - totalDrifFundingRequest - otherFundingSum;

    this.budgetForm.patchValue({ remainingAmount });

    const intendToSecureFunding = this.budgetForm.get('intendToSecureFunding');

    if (remainingAmount > 0) {
      intendToSecureFunding?.addValidators(Validators.required);
    } else {
      intendToSecureFunding?.clearValidators();
      intendToSecureFunding?.reset();
    }

    intendToSecureFunding?.updateValueAndValidity();
  }

  getFormArray(formArrayName: string) {
    return this.budgetForm.get(formArrayName) as FormArray;
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

  removeOtherSource(index: number) {
    this.getFormArray('otherFunding').removeAt(index);
  }

  addOtherFunding() {
    this.getFormArray('otherFunding').push(
      this.formBuilder.formGroup(FundingInformationItemForm)
    );
  }

  getRemainingAmount() {
    return this.budgetForm.get('remainingAmount')?.value;
  }

  getRemainingAmountAbs() {
    return Math.abs(this.getRemainingAmount());
  }
}
