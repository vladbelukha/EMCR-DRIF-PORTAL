import { Component, Input } from '@angular/core';
import { FormArray, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EOIApplicationForm } from '../eoi-application/eoi-application-form';
import { IFormGroup, RxFormControl } from '@rxweb/reactive-form-validators';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { Hazards } from '../../model';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { TranslocoModule } from '@ngneat/transloco';
import { DrrInputComponent } from '../drr-input/drr-input.component';

@Component({
  selector: 'drr-step-2',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatRadioModule,
    MatSelectModule,
    MatDatepickerModule,
    TranslocoModule,
    DrrInputComponent,
  ],
  templateUrl: './step-2.component.html',
  styleUrl: './step-2.component.scss',
})
export class Step2Component {
  @Input()
  eoiApplicationForm!: IFormGroup<EOIApplicationForm>;

  hazardsOptions = Object.values(Hazards);

  getFormArray(formArrayName: string) {
    return this.eoiApplicationForm.get(formArrayName) as FormArray;
  }

  getFormControl(name: string): RxFormControl {
    return this.eoiApplicationForm.get(name) as RxFormControl;
  }

  otherHazardSelected() {
    return this.getFormArray('relatedHazards').value?.includes('Other');
  }
}
