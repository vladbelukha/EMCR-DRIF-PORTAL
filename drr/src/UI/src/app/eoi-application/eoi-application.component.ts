import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatStepperModule } from '@angular/material/stepper';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { ReactiveFormsModule, FormGroup, FormControl, FormsModule, Validators, FormBuilder, FormArray } from '@angular/forms';
import {
  IFormGroup,
  RxFormArray,
  RxFormBuilder,
  RxFormGroup,
  RxwebValidators,
  email,
  prop,
  propArray,
  propObject,
  required,
} from '@rxweb/reactive-form-validators';
import { ApplicantType, ContactDetails, EOIApplication, ProjectType } from '../../model';
import { ContactDetailsForm, EOIApplicationForm } from './eoi-application-form';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { ContactDetailsComponent } from '../contact-details/contact-details.component';

@Component({
  selector: 'drr-eoi-application',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatStepperModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatRadioModule,
    MatIconModule,
    MatDividerModule,
    MatSelectModule,
    MatDatepickerModule,
    ContactDetailsComponent
  ],
  templateUrl: './eoi-application.component.html',
  styleUrl: './eoi-application.component.scss',
  providers: [RxFormBuilder],
})
export class EOIApplicationComponent {
  ApplicantType = ApplicantType;
  ProjectType = ProjectType;

  hazardsOptions = [
    'Drought and water scarcity',
    'Erosion',
    'Extreme Temperature',
    'Flood',
    'Geohazards (e.g., avalanche, landslide)',
    'Sea Level Rise',
    'Seismic',
    'Storm',
    'Tsunami',
    'Other'
  ];

  formBuilder = inject(RxFormBuilder);

  eoiApplicationForm = this.formBuilder.formGroup(
    EOIApplicationForm,
  ) as IFormGroup<EOIApplicationForm>;  

  getFormArray(formArrayName: string) {
    return this.eoiApplicationForm.get(formArrayName) as FormArray;
  }

  // getItemFormGroup(formArrayName: string, index: number) {
  //   return this.getFormArray(formArrayName)[index] as RxFormGroup;
  // }

  addProjectContact() {    
    this.getFormArray('projectContacts').push(this.formBuilder.formGroup(ContactDetailsForm), { emitEvent: false }); // TODO: still makes parent dirty, which is not ideal
  }

  removeProjectContact(index: number) {
    const projectContacts = this.getFormArray('projectContacts').controls;
    projectContacts.splice(index, 1);
  }

  validateFirstStep() {
    this.eoiApplicationForm.controls.applicantType?.markAsDirty();
    this.eoiApplicationForm.controls.applicantName?.markAsDirty();
    this.eoiApplicationForm.controls.submitter?.markAsDirty();
    console.log(this.eoiApplicationForm.value);
  }

  addHazard() {
    const newHazardControl = this.formBuilder.control('', [RxwebValidators.required()]);
    const array = this.getFormArray('relatedHazards');
    array.push(newHazardControl, { emitEvent: false });
  }
}
