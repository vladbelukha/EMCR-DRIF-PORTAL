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
  required,
} from '@rxweb/reactive-form-validators';

export class AppContactForm { // TODO: implements CreateModel
  @prop()
  applicationType!: string;

  @prop()
  primaryApplicantName!: string;
}

@Component({
  selector: 'drif-start-application',
  standalone: true,
  imports: [
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
  formBuilder = inject(RxFormBuilder);

  contactFormGroup = this.formBuilder.formGroup(
    AppContactForm,
  ) as IFormGroup<AppContactForm>;

  // contactFormGroup = this._formBuilder.group({
  //   applicationType: ['', Validators.required],
  //   primaryApplicantName: ['', Validators.required],
  // });
  // secondFormGroup = this._formBuilder.group({
  //   secondCtrl: ['', Validators.required],
  // });
}
