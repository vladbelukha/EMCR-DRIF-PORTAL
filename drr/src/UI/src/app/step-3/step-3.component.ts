import { Component, Input, inject } from '@angular/core';
import {
  EOIApplicationForm,
  FundingInformationForm,
} from '../eoi-application/eoi-application-form';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { FormArray, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { distinctUntilChanged } from 'rxjs';
import { FundingType } from '../../model';
import { MatSelectModule } from '@angular/material/select';
import { TranslocoModule } from '@ngneat/transloco';

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
}
