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
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import {
  IFormGroup,
  RxFormBuilder,
  RxFormControl,
} from '@rxweb/reactive-form-validators';
import { EstimatedNumberOfPeople } from '../../../../model';
import { DrrInputComponent } from '../../../shared/controls/drr-input/drr-input.component';
import { DrrSelectComponent } from '../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import {
  InfrastructureImpactedForm,
  ProjectDetailsForm,
} from '../drif-eoi-form';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drif-eoi-step-5',
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
    DrrSelectComponent,
  ],
  templateUrl: './drif-eoi-step-5.component.html',
  styleUrl: './drif-eoi-step-5.component.scss',
})
export class DrifEoiStep5Component {
  formBuilder = inject(RxFormBuilder);
  translocoService = inject(TranslocoService);

  estimatedNumberOfPeopleOptions = Object.values(EstimatedNumberOfPeople).map(
    (value) => ({
      value,
      label: this.translocoService.translate(value),
    })
  );

  @Input()
  projectDetailsForm!: IFormGroup<ProjectDetailsForm>;

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
    this.getFormArray('infrastructureImpacted').push(
      this.formBuilder.formGroup(InfrastructureImpactedForm)
    );
  }

  removeInfrastructure(index: number) {
    this.getFormArray('infrastructureImpacted').removeAt(index);
  }
}
