import { BreakpointObserver, LayoutModule } from '@angular/cdk/layout';
import {
  STEPPER_GLOBAL_OPTIONS,
  StepperSelectionEvent,
} from '@angular/cdk/stepper';
import { CommonModule } from '@angular/common';
import { Component, inject, ViewChild } from '@angular/core';
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

  stepperOrientation: StepperOrientation = 'vertical';

  autoSaveTimer: any;
  autoSaveCountdown = 0;
  autoSaveInterval = 60;
  lastSavedAt?: Date;
  formChanged = false;

  @ViewChild(MatStepper) stepper!: MatStepper;

  id?: string;
  get isEditMode() {
    return !!this.id;
  }

  drifFpForm = this.formBuilder.formGroup(DrifFpForm) as IFormGroup<DrifFpForm>;

  ngOnInit() {
    this.id = this.route.snapshot.params['id'];

    if (this.isEditMode) {
      this.load();
    }

    this.breakpointObserver
      .observe('(min-width: 768px)')
      .subscribe(({ matches }) => {
        this.stepperOrientation = matches ? 'horizontal' : 'vertical';
      });
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
    };

    this.drifFpForm.patchValue(formData, { emitEvent: false });
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

  save() {}

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
