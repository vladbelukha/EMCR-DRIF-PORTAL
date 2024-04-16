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
  ],
  templateUrl: './step-3.component.html',
  styleUrl: './step-3.component.scss',
})
export class Step3Component {
  @Input()
  eoiApplicationForm!: IFormGroup<EOIApplicationForm>;

  formBuilder = inject(RxFormBuilder);

  getFormArray(formArrayName: string) {
    return this.eoiApplicationForm.get(formArrayName) as FormArray;
  }

  addOtherFunding() {
    this.getFormArray('otherFunding').push(
      this.formBuilder.formGroup(FundingInformationForm),
      { emitEvent: false }
    );
  }
}
