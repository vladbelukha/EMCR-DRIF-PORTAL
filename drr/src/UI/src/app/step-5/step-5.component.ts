import { CommonModule } from '@angular/common';
import { Component, Input, inject } from '@angular/core';
import { FormArray, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import {
  EOIApplicationForm,
  StringItem,
} from '../eoi-application/eoi-application-form';
import { TranslocoModule } from '@ngneat/transloco';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'drr-step-5',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    TranslocoModule,
  ],
  templateUrl: './step-5.component.html',
  styleUrl: './step-5.component.scss',
})
export class Step5Component {
  formBuilder = inject(RxFormBuilder);

  @Input()
  eoiApplicationForm!: IFormGroup<EOIApplicationForm>;

  ngOnInit() {
    this.eoiApplicationForm
      .get('infrastructureImpactedArray')
      ?.valueChanges.subscribe((infrastructures: StringItem[]) => {
        this.eoiApplicationForm
          .get('infrastructureImpacted')
          ?.patchValue(
            infrastructures.map((infrastructure) => infrastructure.value)
          );
      });
  }

  getFormArray(formArrayName: string) {
    return this.eoiApplicationForm.get(formArrayName) as FormArray;
  }

  addInfrastructure() {
    this.getFormArray('infrastructureImpactedArray').push(
      this.formBuilder.formGroup(StringItem)
    );
  }

  removeInfrastructure(index: number) {
    this.getFormArray('infrastructureImpactedArray').removeAt(index);
  }
}
