import { BreakpointObserver, LayoutModule } from '@angular/cdk/layout';
import {
  STEPPER_GLOBAL_OPTIONS,
  StepperSelectionEvent,
} from '@angular/cdk/stepper';
import { CommonModule } from '@angular/common';
import { Component, HostListener, ViewChild, inject } from '@angular/core';
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
  MatStepper,
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
import { distinctUntilChanged } from 'rxjs/operators';
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

  @ViewChild(MatStepper) stepper!: MatStepper;

  private formToStepMap: Record<string, string> = {
    proponentInformation: 'Step 1',
    projectInformation: 'Step 2',
    fundingInformation: 'Step 3',
    locationInformation: 'Step 4',
    projectDetails: 'Step 5',
    engagementPlan: 'Step 6',
    otherSupportingInformation: 'Step 7',
    declaration: 'Step 8',
  };

  @HostListener('window:mousemove')
  @HostListener('window:mousedown')
  @HostListener('window:keypress')
  @HostListener('window:scroll')
  @HostListener('window:touchmove')
  resetAutoSaveTimer() {
    if (!this.formChanged) {
      this.autoSaveCountdown = 0;
      clearInterval(this.autoSaveTimer);
      return;
    }

    this.autoSaveCountdown = 15;
    console.log('reset timer');
    clearInterval(this.autoSaveTimer);
    this.autoSaveTimer = setInterval(() => {
      this.autoSaveCountdown -= 1;
      if (this.autoSaveCountdown === 0) {
        this.save();
        clearInterval(this.autoSaveTimer);
      }
    }, 1000);
  }

  autoSaveTimer: any;
  autoSaveCountdown = 0;
  formChanged = false;

  ngOnInit() {
    this.breakpointObserver
      .observe('(min-width: 768px)')
      .subscribe(({ matches }) => {
        this.stepperOrientation = matches ? 'horizontal' : 'vertical';
      });

    this.eoiApplicationForm.valueChanges
      .pipe(distinctUntilChanged())
      .subscribe(() => {
        this.formChanged = this.eoiApplicationForm.dirty;
        this.resetAutoSaveTimer();
      });
  }

  getFormGroup(groupName: string) {
    return this.eoiApplicationForm?.get(groupName) as RxFormGroup;
  }

  stepperSelectionChange(event: StepperSelectionEvent) {
    this.save();

    event.previouslySelectedStep.stepControl.markAllAsTouched();

    if (this.stepperOrientation === 'horizontal') {
      return;
    }

    const stepId = this.stepper._getStepLabelId(event.selectedIndex);
    const stepElement = document.getElementById(stepId);
    if (stepElement) {
      setTimeout(() => {
        stepElement.scrollIntoView({
          block: 'start',
          inline: 'nearest',
          behavior: 'smooth',
        });
      }, 250);
    }
  }

  // TODO: remove later
  lastSavedAt?: Date;

  save() {
    this.lastSavedAt = new Date();

    const eoiApplicationForm =
      this.eoiApplicationForm.getRawValue() as EOIApplicationForm;

    const drifEoiApplication = {
      ...eoiApplicationForm.proponentInformation,
      ...eoiApplicationForm.projectInformation,
      ...eoiApplicationForm.fundingInformation,
      ...eoiApplicationForm.locationInformation,
      ...eoiApplicationForm.projectDetails,
      ...eoiApplicationForm.engagementPlan,
      ...eoiApplicationForm.otherSupportingInformation,
      ...eoiApplicationForm.declaration,
    } as DrifEoiApplication;

    this.applicationService
      .dRIFApplicationCreateDraftEOIApplication(drifEoiApplication)
      .subscribe(
        (response) => {
          this.hotToast.close();
          this.hotToast.success('Application saved successfully', {
            duration: 5000,
            autoClose: true,
          });

          this.formChanged = false;
        },
        (error) => {
          this.hotToast.close();
          this.hotToast.error('Failed to save application');
        }
      );
  }

  submit() {
    this.eoiApplicationForm.markAllAsTouched();
    this.stepper.steps.forEach((step) => step._markAsInteracted());
    this.stepper._stateChanged();
    if (this.eoiApplicationForm.invalid) {
      // select which forms are invalid
      const invalidSteps = Object.keys(this.eoiApplicationForm.controls)
        .filter((key) => this.eoiApplicationForm.get(key)?.invalid)
        .map((key) => this.formToStepMap[key]);

      const lastStep = invalidSteps.pop();

      const stepsErrorMessage =
        invalidSteps.length > 0
          ? `${invalidSteps.join(', ')} and ${lastStep}`
          : lastStep;

      this.hotToast.close();
      this.hotToast.error(
        `Please fill all the required fields in ${stepsErrorMessage}.`
      );

      return;
    }

    const eoiApplicationForm =
      this.eoiApplicationForm.getRawValue() as EOIApplicationForm;

    const drifEoiApplication = {
      ...eoiApplicationForm.proponentInformation,
      ...eoiApplicationForm.projectInformation,
      ...eoiApplicationForm.fundingInformation,
      ...eoiApplicationForm.locationInformation,
      ...eoiApplicationForm.projectDetails,
      ...eoiApplicationForm.engagementPlan,
      ...eoiApplicationForm.otherSupportingInformation,
      ...eoiApplicationForm.declaration,
    } as DrifEoiApplication;

    this.applicationService
      .dRIFApplicationCreateEOIApplication(drifEoiApplication)
      .subscribe(
        (response) => {
          this.hotToast.close();
          this.router.navigate(['/success']);
        },
        (error) => {
          this.hotToast.close();
          this.hotToast.error('Failed to submit application');
        }
      );
  }
}
