import { BreakpointObserver, LayoutModule } from '@angular/cdk/layout';
import {
  STEPPER_GLOBAL_OPTIONS,
  StepperSelectionEvent,
} from '@angular/cdk/stepper';
import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import {
  MatStepperModule,
  StepperOrientation,
} from '@angular/material/stepper';
import { Router } from '@angular/router';
import { HotToastService } from '@ngneat/hot-toast';
import { TranslocoModule } from '@ngneat/transloco';
import {
  IFormGroup,
  RxFormBuilder,
  RxFormGroup,
} from '@rxweb/reactive-form-validators';
import { DrifapplicationService } from '../../api/drifapplication/drifapplication.service';
import { DrifEoiApplication, Hazards } from '../../model';
import { Step1Component } from '../step-1/step-1.component';
import { Step2Component } from '../step-2/step-2.component';
import { Step3Component } from '../step-3/step-3.component';
import { Step4Component } from '../step-4/step-4.component';
import { Step5Component } from '../step-5/step-5.component';
import { Step6Component } from '../step-6/step-6.component';
import { Step7Component } from '../step-7/step-7.component';
import { Step8Component } from '../step-8/step-8.component';
import { EOIApplicationForm } from './eoi-application-form';

@Component({
  selector: 'drr-eoi-application',
  standalone: true,
  imports: [
    CommonModule,
    LayoutModule,
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
  providers: [
    RxFormBuilder,
    HotToastService,
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true },
    },
  ],
})
export class EOIApplicationComponent {
  isDevMode = false; // isDevMode();
  stepperOrientation: StepperOrientation = 'vertical';

  hazardsOptions = Object.values(Hazards);

  formBuilder = inject(RxFormBuilder);
  applicationService = inject(DrifapplicationService);
  router = inject(Router);
  hotToast = inject(HotToastService);
  breakpointObserver = inject(BreakpointObserver);

  eoiApplicationForm = this.formBuilder.formGroup(
    EOIApplicationForm
  ) as IFormGroup<EOIApplicationForm>;

  ngOnInit() {
    this.breakpointObserver
      .observe('(min-width: 800px)')
      .subscribe(({ matches }) => {
        this.stepperOrientation = matches ? 'horizontal' : 'vertical';
      });
  }

  getFormGroup(groupName: string) {
    return this.eoiApplicationForm?.get(groupName) as RxFormGroup;
  }

  stepperSelectionChange(event: StepperSelectionEvent) {
    event.previouslySelectedStep.stepControl.markAllAsTouched();
  }

  submit() {
    this.eoiApplicationForm.markAllAsTouched();
    if (this.eoiApplicationForm.invalid) {
      this.hotToast.error('Please fill all the required fields');
      return;
    }

    const eoiApplicationForm =
      this.eoiApplicationForm.getRawValue() as EOIApplicationForm;

    const drifEoiApplication = {
      ...eoiApplicationForm,
      ...eoiApplicationForm.proponentInformation,
      ...eoiApplicationForm.projectInformation,
      ...eoiApplicationForm.fundingInformation,
      ...eoiApplicationForm.locationInformation,
      ...eoiApplicationForm.projectDetails,
      ...eoiApplicationForm.engagementPlan,
      ...eoiApplicationForm.otherSupportingInformation,
    } as DrifEoiApplication;

    this.applicationService
      .dRIFApplicationCreateEOIApplication(drifEoiApplication)
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
