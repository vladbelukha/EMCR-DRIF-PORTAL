import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import {
  MatStepperModule,
  StepperOrientation,
} from '@angular/material/stepper';
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import {
  IFormGroup,
  RxFormBuilder,
  RxReactiveFormsModule,
} from '@rxweb/reactive-form-validators';
import {
  ActivityType,
  ProgressReport,
  WorkplanStatus,
  YesNoOption,
} from '../../../../../model';

import { AbstractControl, FormArray, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectService } from '../../../../../api/project/project.service';
import { DrrDatepickerComponent } from '../../../../shared/controls/drr-datepicker/drr-datepicker.component';
import { DrrInputComponent } from '../../../../shared/controls/drr-input/drr-input.component';
import {
  DrrRadioButtonComponent,
  RadioOption,
} from '../../../../shared/controls/drr-radio-button/drr-radio-button.component';
import {
  DrrSelectComponent,
  DrrSelectOption,
} from '../../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../../shared/controls/drr-textarea/drr-textarea.component';
import {
  EventForm,
  EventProgressType,
  ProgressReportForm,
  WorkplanActivityForm,
  WorkplanForm,
} from '../drif-progress-report-form';

@Component({
  selector: 'drr-drif-progress-report-create',
  standalone: true,
  imports: [
    CommonModule,
    MatStepperModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    MatCardModule,
    TranslocoModule,
    DrrDatepickerComponent,
    DrrInputComponent,
    DrrSelectComponent,
    DrrRadioButtonComponent,
    DrrTextareaComponent,
    RxReactiveFormsModule,
  ],
  templateUrl: './drif-progress-report-create.component.html',
  styleUrl: './drif-progress-report-create.component.scss',
  providers: [RxFormBuilder],
})
export class DrifProgressReportCreateComponent {
  formBuilder = inject(RxFormBuilder);
  route = inject(ActivatedRoute);
  router = inject(Router);
  projectService = inject(ProjectService);
  translocoService = inject(TranslocoService);

  projectId!: string;
  reportId!: string;
  progressReportId!: string;

  activityTypeOptions: DrrSelectOption[] = Object.values(ActivityType).map(
    (value) => ({
      label: this.translocoService.translate(`activityType.${value}`),
      value,
    })
  );

  optionalActivityOptions: DrrSelectOption[] = Object.values(
    WorkplanStatus
  ).map((value) => ({
    label: this.translocoService.translate(`workplanStatus.${value}`),
    value,
  }));

  necessaryActivityOptions: RadioOption[] = this.optionalActivityOptions.filter(
    (option) => option.value !== WorkplanStatus.NoLongerNeeded
  );

  yesNoNaRadioOptions: RadioOption[] = Object.values(YesNoOption).map(
    (value) => ({
      label: value, // TODO: translate
      value,
    })
  );

  yesNoRadioOptions: RadioOption[] = [
    {
      label: 'Yes',
      value: true,
    },
    {
      label: 'No',
      value: false,
    },
  ];

  yesNoSelectOptions: DrrSelectOption[] = [
    {
      label: 'Yes',
      value: YesNoOption.Yes,
    },
    {
      label: 'No',
      value: YesNoOption.No,
    },
  ];

  eventProgressOptions: RadioOption[] = Object.values(EventProgressType).map(
    (value) => ({
      label: value,
      value,
    })
  );

  stepperOrientation: StepperOrientation = 'horizontal';

  progressReportForm = this.formBuilder.formGroup(
    ProgressReportForm
  ) as IFormGroup<ProgressReportForm>;

  get workplanForm(): IFormGroup<WorkplanForm> | null {
    return this.progressReportForm.get('workplan') as IFormGroup<WorkplanForm>;
  }

  get workplanItems(): FormArray | null {
    return this.workplanForm?.get('workplanActivities') as FormArray;
  }

  get eventForm(): IFormGroup<EventForm> | null {
    return this.progressReportForm.get('event') as IFormGroup<EventForm>;
  }

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = params['projectId'];
      this.reportId = params['reportId'];
      this.progressReportId = params['progressReportId'];

      this.projectService
        .projectGetProgressReport(
          this.projectId,
          this.reportId,
          this.progressReportId
        )
        .subscribe((report: ProgressReport) => {
          this.progressReportForm.patchValue(report);

          report.workplan?.workplanActivities?.map((activity) => {
            // TODO: remove after API fills the values
            activity.isMandatory = activity.isMandatory ?? true;

            const activityForm = this.formBuilder.formGroup(
              new WorkplanActivityForm(activity)
            );

            this.workplanItems?.push(activityForm);
          });

          this.progressReportForm
            .get('workplan.fundingSourcesChanged')
            ?.valueChanges.subscribe((value) => {
              const comment = this.progressReportForm.get(
                'workplan.fundingSourcesChangedComment'
              );

              if (value) {
                comment?.addValidators(Validators.required);
              } else {
                comment?.removeValidators(Validators.required);
              }

              comment?.updateValueAndValidity();
            });
        });
    });
  }

  stepperSelectionChange(event: any) {}

  save() {}

  goBack() {
    // TODO: save

    this.router.navigate(['drif-projects', this.projectId]);
  }

  submit() {}

  getPreDefinedActivitiesArray() {
    return this.workplanItems?.controls.filter(
      (control) => control.get('preCreatedActivity')?.value
    );
  }

  getAdditionalActivitiesArray() {
    return this.workplanItems?.controls
      .filter((control) => !control.get('preCreatedActivity')?.value)
      .sort((a, b) => {
        const aMandatory = a.get('isMandatory')?.value;
        const bMandatory = b.get('isMandatory')?.value;

        if (aMandatory && !bMandatory) {
          return -1;
        }

        if (!aMandatory && bMandatory) {
          return 1;
        }

        return 0;
      });
  }

  addAdditionalActivity() {
    this.workplanItems?.push(
      this.formBuilder.formGroup(
        new WorkplanActivityForm({
          isMandatory: false,
        })
      )
    );
  }

  getAdditionalActivityOptions(activityControl: AbstractControl) {
    if (!this.isAdditionalActivityMandatory(activityControl)) {
      return this.necessaryActivityOptions;
    }

    return this.optionalActivityOptions;
  }

  isAdditionalActivityMandatory(activityControl: AbstractControl) {
    return !!activityControl.get('isMandatory')?.value;
  }

  removeAdditionalActivity(index: number) {
    this.workplanItems?.removeAt(index);
  }

  showPlannedStartDate(activityControl: AbstractControl<WorkplanActivityForm>) {
    const status = activityControl?.get('status')?.value as WorkplanStatus;
    return status === WorkplanStatus.NotStarted;
  }

  showPlannedEndDate(activityControl: AbstractControl<WorkplanActivityForm>) {
    const status = activityControl?.get('status')?.value as WorkplanStatus;
    return (
      status === WorkplanStatus.NotStarted ||
      status === WorkplanStatus.InProgress
    );
  }

  showActualStartDate(activityControl: AbstractControl<WorkplanActivityForm>) {
    const status = activityControl?.get('status')?.value as WorkplanStatus;
    return (
      status === WorkplanStatus.InProgress ||
      status === WorkplanStatus.Completed
    );
  }

  showActualEndDate(activityControl: AbstractControl<WorkplanActivityForm>) {
    const status = activityControl?.get('status')?.value as WorkplanStatus;
    return status === WorkplanStatus.Completed;
  }

  showFundingSourcesChangedComment() {
    return this.progressReportForm
      .get('workplan.fundingSourcesChangedComment')
      ?.hasValidator(Validators.required);
  }
}
