import { CommonModule } from '@angular/common';
import { Component, Input, inject } from '@angular/core';
import {
  FormArray,
  FormControl,
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
import {
  IFormGroup,
  RxFormBuilder,
  RxFormControl,
} from '@rxweb/reactive-form-validators';
import { distinctUntilChanged } from 'rxjs';
import { FundingType } from '../../model';
import { DrrTextareaComponent } from '../drr-datepicker/drr-textarea.component';
import { DrrInputComponent } from '../drr-input/drr-input.component';
import {
  FundingInformationForm,
  FundingInformationItemForm,
} from '../eoi-application/eoi-application-form';

@Component({
  selector: 'drr-step-3',
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
  ],
  templateUrl: './step-3.component.html',
  styleUrl: './step-3.component.scss',
})
export class Step3Component {
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

    this.fundingInformationForm.get('remainingAmount')?.disable();
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
    remainingAmount = remainingAmount < 0 ? 0 : remainingAmount;

    this.fundingInformationForm.patchValue({ remainingAmount });

    const intendToSecureFunding = this.fundingInformationForm.get(
      'intendToSecureFunding'
    );

    if (remainingAmount > 0) {
      intendToSecureFunding?.addValidators(Validators.required);
    } else {
      intendToSecureFunding?.clearValidators();
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

  getFormControl(name: string): FormControl {
    return this.fundingInformationForm.get(name) as FormControl;
  }

  getArrayFormControl(
    controlName: string,
    arrayName: string,
    index: number
  ): RxFormControl {
    return this.getFormArray(arrayName)?.controls[index]?.get(
      controlName
    ) as RxFormControl;
  }
}
