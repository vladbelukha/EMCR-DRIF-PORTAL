import { BreakpointObserver, LayoutModule } from '@angular/cdk/layout';
import {
  STEPPER_GLOBAL_OPTIONS,
  StepperSelectionEvent,
} from '@angular/cdk/stepper';
import { CommonModule } from '@angular/common';
import { Component, HostListener, inject, ViewChild } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import {
  MatStepper,
  MatStepperModule,
  StepperOrientation,
} from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { HotToastService } from '@ngneat/hot-toast';
import { TranslocoModule } from '@ngneat/transloco';
import {
  IFormGroup,
  RxFormBuilder,
  RxFormGroup,
} from '@rxweb/reactive-form-validators';
import { DrifFpStep1Component } from '../drif-fp-step-1/drif-fp-step-1.component';

import { distinctUntilChanged } from 'rxjs/operators';
import { DrifapplicationService } from '../../../api/drifapplication/drifapplication.service';
import { DraftFpApplication } from '../../../model';
import { DrifFpStep5Component } from '../drif-fp-step-5/drif-fp-step-5.component';
import { DrifFpStep2Component } from '../drif-fp-step-2/drif-fp-step-2.component';
import { DrifFpForm } from './drif-fp-form';

@Component({
  selector: 'drr-drif-fp',
  standalone: true,
  imports: [
    CommonModule,
    MatStepperModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatIconModule,
    TranslocoModule,
    LayoutModule,
    DrifFpStep1Component,
    DrifFpStep2Component,
    DrifFpStep5Component,
  ],
  providers: [
    RxFormBuilder,
    HotToastService,
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true },
    },
  ],
  templateUrl: './drif-fp.component.html',
  styleUrl: './drif-fp.component.scss',
})
export class DrifFpComponent {
  formBuilder = inject(RxFormBuilder);
  breakpointObserver = inject(BreakpointObserver);
  router = inject(Router);
  route = inject(ActivatedRoute);
  appService = inject(DrifapplicationService);
  hotToast = inject(HotToastService);

  stepperOrientation: StepperOrientation = 'vertical';

  autoSaveTimer: any;
  autoSaveCountdown = 0;
  autoSaveInterval = 60;
  lastSavedAt?: Date;
  formChanged = false;

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

    this.autoSaveCountdown = this.autoSaveInterval;
    clearInterval(this.autoSaveTimer);
    this.autoSaveTimer = setInterval(() => {
      this.autoSaveCountdown -= 1;
      if (this.autoSaveCountdown === 0) {
        this.save();
        clearInterval(this.autoSaveTimer);
      }
    }, 1000);
  }

  @ViewChild(MatStepper) stepper!: MatStepper;

  id?: string;
  get isEditMode() {
    return !!this.id;
  }

  drifFpForm = this.formBuilder.formGroup(DrifFpForm) as IFormGroup<DrifFpForm>;

  ngOnInit() {
    this.breakpointObserver
      .observe('(min-width: 768px)')
      .subscribe(({ matches }) => {
        this.stepperOrientation = matches ? 'horizontal' : 'vertical';
      });

    this.id = this.route.snapshot.params['id'];

    if (this.isEditMode) {
      this.load();
      this.formChanged = false;
    }

    setTimeout(() => {
      this.drifFpForm.valueChanges
        .pipe(
          distinctUntilChanged((a, b) => {
            return JSON.stringify(a) == JSON.stringify(b);
          })
        )
        .subscribe(() => {
          this.formChanged = true;
          this.resetAutoSaveTimer();
        });
    }, 1000);
  }

  load() {
    const formData: DrifFpForm = {
      eoiId: 'DRIF-EOI-1111',
      fundingStream: 'Stream1',
      projectType: 'Existing',
      proponentInformation: {
        proponentType: 'FirstNation',
        projectContact: {
          firstName: 'Jane',
          lastName: 'Doe',
          email: 'asd@.asda.asd',
          phone: '123-456-7890',
          department: 'IT',
          title: 'Ms.',
        },
        additionalContacts: [
          {
            firstName: 'John1',
            lastName: 'Doe1',
            email: 'jd1@exmapl.as',
            phone: '123-456-7890',
            department: 'IT1',
            title: 'Mr.1',
          },
          {
            firstName: 'John2',
            lastName: 'Doe2',
            email: 'jd2@exmapl.as',
            phone: '123-456-7890',
            department: 'IT2',
            title: 'Mr.2',
          },
        ],
      },
      budget: {
        totalProjectCost: 1304020,
      },
    };

    this.drifFpForm.patchValue(formData, { emitEvent: false });
    this.drifFpForm.markAsPristine();
  }

  getFormGroup(groupName: string) {
    return this.drifFpForm?.get(groupName) as RxFormGroup;
  }

  getFundingStream() {
    return this.drifFpForm?.get('fundingStream')?.value == 'Stream1'
      ? 'shortStream1'
      : 'shortStream2';
  }

  getPrimaryProponent() {
    return this.drifFpForm?.get('proponentInformation')?.get('proponentName')
      ?.value;
  }

  getProjectType() {
    return this.drifFpForm?.get('projectType')?.value == 'Existing'
      ? 'shortExisting'
      : 'new';
  }

  getRelatedEOI() {
    return this.drifFpForm?.get('eoiId')?.value;
  }

  goBack() {
    this.router.navigate(['/dashboard']);
  }

  save() {
    if (!this.formChanged) {
      return;
    }

    const drifFpForm = this.drifFpForm.getRawValue() as DrifFpForm;

    const fpDraft = {
      // initialize draft object from form data
    } as DraftFpApplication;

    this.lastSavedAt = undefined;
    this.appService
      .dRIFApplicationUpdateFPApplication(this.id!, fpDraft)
      .subscribe(
        (response) => {
          this.lastSavedAt = new Date();

          this.hotToast.close();
          this.hotToast.success('Application saved successfully', {
            duration: 5000,
            autoClose: true,
          });

          this.formChanged = false;
          this.resetAutoSaveTimer();
        },
        (error) => {
          this.hotToast.close();
          this.hotToast.error('Failed to save application');
        }
      );
  }

  submit() {}

  stepperSelectionChange(event: StepperSelectionEvent) {
    if (this.isEditMode) {
      // this.save();
    }

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
}
