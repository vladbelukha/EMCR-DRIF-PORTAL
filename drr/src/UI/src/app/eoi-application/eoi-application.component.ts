import { Component, ViewChild, inject, isDevMode } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
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
  ProponentType,
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
import { HotToastService } from '@ngneat/hot-toast';

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
  providers: [RxFormBuilder, HotToastService],
})
export class EOIApplicationComponent {
  isDevMode = false; // isDevMode();

  hazardsOptions = Object.values(Hazards);

  formBuilder = inject(RxFormBuilder);
  applicationService = inject(DrifapplicationService);
  router = inject(Router);
  hotToast = inject(HotToastService);

  eoiApplicationForm = this.formBuilder.formGroup(
    EOIApplicationForm
  ) as IFormGroup<EOIApplicationForm>;

  @ViewChild('stepper') stepper: MatStepper | undefined;

  getFormArray(formArrayName: string) {
    return this.eoiApplicationForm?.get(formArrayName) as FormArray;
  }

  validateStep1() {
    this.eoiApplicationForm.get('proponentType')?.markAsTouched();
    this.eoiApplicationForm.get('proponentName')?.markAsTouched();
    this.eoiApplicationForm.get('submitter')?.markAllAsTouched();
    this.eoiApplicationForm.get('projectContact')?.markAllAsTouched();
    this.eoiApplicationForm.get('additionalContacts')?.markAllAsTouched();
    this.eoiApplicationForm.get('partneringProponents')?.markAllAsTouched();

    if (
      this.eoiApplicationForm.get('proponentType')?.valid &&
      this.eoiApplicationForm.get('proponentName')?.valid &&
      this.eoiApplicationForm.get('submitter')?.valid &&
      this.eoiApplicationForm.get('projectContact')?.valid
    ) {
      this.stepper?.next();
    }
  }

  validateStep2() {
    this.eoiApplicationForm.get('fundingStream')?.markAsTouched();
    this.eoiApplicationForm.get('projectTitle')?.markAsTouched();
    this.eoiApplicationForm.get('scopeStatement')?.markAsTouched();
    this.eoiApplicationForm.get('projectType')?.markAsTouched();
    this.eoiApplicationForm.get('relatedHazards')?.markAsTouched();
    this.eoiApplicationForm.get('startDate')?.markAsTouched();
    this.eoiApplicationForm.get('endDate')?.markAsTouched();

    if (
      this.eoiApplicationForm.get('fundingStream')?.valid &&
      this.eoiApplicationForm.get('projectTitle')?.valid &&
      this.eoiApplicationForm.get('scopeStatement')?.valid &&
      this.eoiApplicationForm.get('projectType')?.valid &&
      this.eoiApplicationForm.get('relatedHazards')?.valid &&
      this.eoiApplicationForm.get('startDate')?.valid &&
      this.eoiApplicationForm.get('endDate')?.valid
    ) {
      this.stepper?.next();
    }
  }

  validateStep3() {
    this.eoiApplicationForm.get('estimatedTotal')?.markAsTouched();
    this.eoiApplicationForm.get('fundingRequest')?.markAsTouched();
    this.eoiApplicationForm.get('otherFunding')?.markAllAsTouched();
    this.eoiApplicationForm.get('intendToSecureFunding')?.markAsTouched();

    if (
      this.eoiApplicationForm.get('estimatedTotal')?.valid &&
      this.eoiApplicationForm.get('fundingRequest')?.valid &&
      this.eoiApplicationForm.get('otherFunding')?.valid &&
      this.eoiApplicationForm.get('intendToSecureFunding')?.valid
    ) {
      this.stepper?.next();
    }
  }

  validateStep4() {
    this.eoiApplicationForm.get('ownershipDeclaration')?.markAsDirty();
    this.eoiApplicationForm.get('locationDescription')?.markAsDirty();
  }

  validateStep5() {
    this.eoiApplicationForm.get('rationaleForFunding')?.markAsDirty();
    this.eoiApplicationForm.get('estimatedPeopleImpacted')?.markAsDirty();
    this.eoiApplicationForm.get('infrastructureImpacted')?.markAsDirty();
    this.eoiApplicationForm.get('disasterRiskUnderstanding')?.markAsDirty();
    this.eoiApplicationForm.get('addressRisksAndHazards')?.markAsDirty();
    this.eoiApplicationForm.get('drifProgramGoalAlignment')?.markAsDirty();
    this.eoiApplicationForm.get('additionalSolutionInformation')?.markAsDirty();
    this.eoiApplicationForm.get('rationaleForSolution')?.markAsDirty();
  }

  validateStep6() {
    this.eoiApplicationForm.get('otherFunding')?.markAsDirty();
  }

  validateStep7() {
    this.eoiApplicationForm.get('climateAdaptation')?.markAsDirty();
  }

  validateStep8() {
    this.eoiApplicationForm.markAllAsTouched();
    if (this.eoiApplicationForm.invalid) {
      this.hotToast.error('Please fill all the required fields');
      return;
    }

    const applicationModel =
      this.eoiApplicationForm.getRawValue() as DrifEoiApplication;

    this.applicationService
      .dRIFApplicationCreateEOIApplication(applicationModel)
      .subscribe(
        (response) => {
          this.router.navigate(['/success']);
        },
        (error) => {
          this.hotToast.error('Failed to submit application');
        }
      );
  }
}
