import { Component, Input, inject } from '@angular/core';
import {
  EOIApplicationForm,
  FundingInformationForm,
} from '../eoi-application/eoi-application-form';
import {
  IFormGroup,
  RxFormBuilder,
  RxFormControl,
  RxwebValidators,
} from '@rxweb/reactive-form-validators';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import {
  FormArray,
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { distinctUntilChanged } from 'rxjs';
import { FundingType } from '../../model';
import { MatSelectModule } from '@angular/material/select';
import { TranslocoModule } from '@ngneat/transloco';
import { DrrTextareaComponent } from '../drr-datepicker/drr-textarea.component';
import { DrrInputComponent } from '../drr-input/drr-input.component';

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
  eoiApplicationForm!: IFormGroup<EOIApplicationForm>;

  fundingTypeOptions = Object.values(FundingType);

  formBuilder = inject(RxFormBuilder);

  ngOnInit() {
    this.eoiApplicationForm
      .get('otherFunding')!
      .valueChanges.pipe(distinctUntilChanged())
      .subscribe(() => {
        this.calculateRemainingAmount();
      });

    this.eoiApplicationForm
      .get('fundingRequest')!
      .valueChanges.pipe(distinctUntilChanged())
      .subscribe(() => {
        this.calculateRemainingAmount();
      });

    this.eoiApplicationForm
      .get('estimatedTotal')!
      .valueChanges.pipe(distinctUntilChanged())
      .subscribe(() => {
        this.calculateRemainingAmount();
      });

    this.eoiApplicationForm.get('remainingAmount')?.disable();
  }

  calculateRemainingAmount() {
    const estimatedTotal =
      this.eoiApplicationForm.get('estimatedTotal')?.value ?? 0;

    const otherFundingSum = this.getFormArray('otherFunding').controls.reduce(
      (total, funding) => total + funding.value.amount,
      0
    );

    const fundingRequest =
      this.eoiApplicationForm.get('fundingRequest')?.value ?? 0;

    let remainingAmount = estimatedTotal - otherFundingSum - fundingRequest;
    remainingAmount = remainingAmount < 0 ? 0 : remainingAmount;

    this.eoiApplicationForm.patchValue({ remainingAmount });

    const intendToSecureFunding = this.eoiApplicationForm.get(
      'intendToSecureFunding'
    );

    if (remainingAmount > 0) {
      intendToSecureFunding?.addValidators(Validators.required);
    } else {
      intendToSecureFunding?.removeValidators(Validators.required);
    }

    intendToSecureFunding?.updateValueAndValidity();
  }

  getFormArray(formArrayName: string) {
    return this.eoiApplicationForm.get(formArrayName) as FormArray;
  }

  addOtherFunding() {
    this.getFormArray('otherFunding').push(
      this.formBuilder.formGroup(FundingInformationForm)
    );
  }

  removeOtherSource(index: number) {
    this.getFormArray('otherFunding').removeAt(index);
  }

  getFormControl(name: string): FormControl {
    return this.eoiApplicationForm.get(name) as FormControl;
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
