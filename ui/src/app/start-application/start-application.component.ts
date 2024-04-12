import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatStepperModule } from '@angular/material/stepper';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatRadioModule } from '@angular/material/radio';
import { ReactiveFormsModule, FormGroup, FormControl, FormsModule, Validators, FormBuilder } from '@angular/forms';
import {
  IFormGroup,
  RxFormBuilder,
  email,
  prop,
  propArray,
  propObject,
  required,
} from '@rxweb/reactive-form-validators';
import { ApplicantType, ContactDetails, EOIApplication, ProjectType } from '../../model';
import { EOIApplicationForm } from './eoi-application-form';

@Component({
  selector: 'drif-start-application',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatStepperModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatRadioModule
  ],
  templateUrl: './start-application.component.html',
  styleUrl: './start-application.component.scss',
  providers: [RxFormBuilder],
})
export class StartApplicationComponent {
  ApplicantType = ApplicantType;

  formBuilder = inject(RxFormBuilder);

  eoiApplicationForm = this.formBuilder.formGroup(
    EOIApplicationForm,
  ) as IFormGroup<EOIApplicationForm>;
}
