import { Component, inject, isDevMode } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatStepperModule } from '@angular/material/stepper';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatCheckboxModule } from '@angular/material/checkbox';
import {
  ReactiveFormsModule,
  FormGroup,
  FormControl,
  FormsModule,
  Validators,
  FormBuilder,
  FormArray,
} from '@angular/forms';
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
import {
  ApplicantType,
  ContactDetails,
  DrifEoiApplication,
  Hazards,
  ProjectType,
} from '../../model';
import {
  ContactDetailsForm,
  EOIApplicationForm,
  FundingInformationForm,
} from './eoi-application-form';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { ContactDetailsComponent } from '../contact-details/contact-details.component';
import { Step1Component } from '../step-1/step-1.component';
import { Step6Component } from '../step-6/step-6.component';
import { Step2Component } from '../step-2/step-2.component';
import { Step3Component } from '../step-3/step-3.component';
import { Step4Component } from '../step-4/step-4.component';
import { Step5Component } from '../step-5/step-5.component';
import { Step7Component } from '../step-7/step-7.component';
import { Step8Component } from '../step-8/step-8.component';
import { DrifapplicationService } from '../../api/drifapplication/drifapplication.service';
import { Router } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';

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
    MatCheckboxModule,
    ContactDetailsComponent,
    Step1Component,
    Step2Component,
    Step3Component,
    Step4Component,
    Step5Component,
    Step6Component,
    Step7Component,
    Step8Component,
    TranslocoModule,
  ],
  templateUrl: './eoi-application.component.html',
  styleUrl: './eoi-application.component.scss',
  providers: [RxFormBuilder],
})
export class EOIApplicationComponent {
  isDevMode = isDevMode();

  ApplicantType = ApplicantType;
  projectType = ProjectType;
  hazardsOptions = Object.values(Hazards);

  formBuilder = inject(RxFormBuilder);
  applicationService = inject(DrifapplicationService);
  router = inject(Router);

  eoiApplicationForm = this.formBuilder.formGroup(
    EOIApplicationForm
  ) as IFormGroup<EOIApplicationForm>;

  getFormArray(formArrayName: string) {
    return this.eoiApplicationForm?.get(formArrayName) as FormArray;
  }

  validateStep1() {
    this.eoiApplicationForm.get('applicantType')?.markAsDirty();
    this.eoiApplicationForm.get('projectTitle')?.markAsDirty();
    this.eoiApplicationForm.get('submitter')?.markAsDirty();
    this.eoiApplicationForm.get('projectContacts')?.markAsDirty();
  }

  validateStep2() {
    this.eoiApplicationForm.get('projectTitle')?.markAsDirty();
    this.eoiApplicationForm.get('projectType')?.markAsDirty();
    this.eoiApplicationForm.get('relatedHazards')?.markAsDirty();
    this.eoiApplicationForm.get('startDate')?.markAsDirty();
    this.eoiApplicationForm.get('endDate')?.markAsDirty();
  }

  validateStep3() {
    this.eoiApplicationForm.get('fundingRequest')?.markAsDirty();
  }

  validateStep4() {
    this.eoiApplicationForm.get('ownershipDeclaration')?.markAsDirty();
    this.eoiApplicationForm.get('locationDescription')?.markAsDirty();
  }

  validateStep5() {
    this.eoiApplicationForm.get('backgroundDescription')?.markAsDirty();
    this.eoiApplicationForm.get('rationaleForFunding')?.markAsDirty();
    this.eoiApplicationForm.get('rationaleForSolution')?.markAsDirty();
    this.eoiApplicationForm.get('proposedSolution')?.markAsDirty();
  }

  validateStep6() {
    this.eoiApplicationForm.get('otherFunding')?.markAsDirty();
  }

  validateStep7() {
    this.eoiApplicationForm.get('climateAdaptation')?.markAsDirty();
  }

  validateStep8() {
    if (this.eoiApplicationForm.invalid) {
      // list invalid fields
      const invalidFields = Object.keys(
        this.eoiApplicationForm.controls
      ).filter((control) => this.eoiApplicationForm.get(control)?.invalid);
      console.log('Invalid fields:', invalidFields);
      // TODO: show toast?
      return;
    }

    const applicationModel = this.eoiApplicationForm
      .value as DrifEoiApplication;

    this.applicationService
      .dRIFApplicationCreateEOIApplication(applicationModel)
      .subscribe(
        (response) => {
          this.router.navigate(['/success']);
        },
        (error) => {
          console.error('Error', error);
        }
      );
  }
}
