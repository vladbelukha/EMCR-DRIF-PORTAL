import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormArray, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup, RxFormControl } from '@rxweb/reactive-form-validators';
import { Hazards } from '../../model';
import { DrrInputComponent } from '../drr-input/drr-input.component';
import { ProjectInformationForm } from '../eoi-application/eoi-application-form';

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
  projectInformationForm!: IFormGroup<ProjectInformationForm>;

  minStartDate = new Date();
  minEndDate = new Date();

  hazardsOptions = Object.values(Hazards);

  ngOnInit() {
    this.projectInformationForm
      .get('startDate')
      ?.valueChanges.subscribe((date) => {
        this.minEndDate = date;
      });
  }

  getFormArray(formArrayName: string) {
    return this.projectInformationForm.get(formArrayName) as FormArray;
  }

  getFormControl(name: string): RxFormControl {
    return this.projectInformationForm.get(name) as RxFormControl;
  }

  otherHazardSelected() {
    return this.getFormArray('relatedHazards').value?.includes('Other');
  }
}
