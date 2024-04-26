import { CommonModule } from '@angular/common';
import { Component, Input, inject } from '@angular/core';
import {
  FormArray,
  FormControl,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import {
  IFormGroup,
  RxFormBuilder,
  RxFormControl,
} from '@rxweb/reactive-form-validators';
import {
  EOIApplicationForm,
  ProjectDetailsForm,
  StringItem,
} from '../eoi-application/eoi-application-form';
import { TranslocoModule } from '@ngneat/transloco';
import { MatIconModule } from '@angular/material/icon';
import { DrrTextareaComponent } from '../drr-datepicker/drr-textarea.component';
import { DrrInputComponent } from '../drr-input/drr-input.component';

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
    DrrTextareaComponent,
    DrrInputComponent,
  ],
  templateUrl: './step-5.component.html',
  styleUrl: './step-5.component.scss',
})
export class Step5Component {
  formBuilder = inject(RxFormBuilder);

  @Input()
  projectDetailsForm!: IFormGroup<ProjectDetailsForm>;

  ngOnInit() {
    this.projectDetailsForm
      .get('infrastructureImpactedArray')
      ?.valueChanges.subscribe((infrastructures: StringItem[]) => {
        this.projectDetailsForm
          .get('infrastructureImpacted')
          ?.patchValue(
            infrastructures.map((infrastructure) => infrastructure.value)
          );
      });
  }

  getFormArray(formArrayName: string) {
    return this.projectDetailsForm.get(formArrayName) as FormArray;
  }

  getFormControl(name: string): FormControl {
    return this.projectDetailsForm.get(name) as FormControl;
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

  addInfrastructure() {
    this.getFormArray('infrastructureImpactedArray').push(
      this.formBuilder.formGroup(StringItem)
    );
  }

  removeInfrastructure(index: number) {
    this.getFormArray('infrastructureImpactedArray').removeAt(index);
  }
}
