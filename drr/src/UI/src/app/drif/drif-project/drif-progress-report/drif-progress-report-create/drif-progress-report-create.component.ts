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
import { ActivityType, YesNoOption } from '../../../../../model';

import { AbstractControl, FormArray } from '@angular/forms';
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
  WorkplanProgressType,
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

  progressReportOptions: RadioOption[] = Object.values(
    WorkplanProgressType
  ).map((value) => ({
    label: value,
    value,
  }));

  yesNoNaOptions = Object.values(YesNoOption).map((value) => ({
    label: value, // TODO: translate
    value,
  }));

  yesNoOptions: DrrSelectOption[] = [
    {
      label: 'Yes',
      value: 'yes',
    },
    {
      label: 'No',
      value: 'no',
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
        .subscribe((report) => {
          this.progressReportForm.patchValue(report);

          // // TODO: temporarily add workplan items
          // report.workplanActivities?.map((activity) => {
          //   this.workplanItems?.push(
          //     this.formBuilder.formGroup(new WorkplanActivityForm(activity))
          //   );
          // });
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
    return this.workplanItems?.controls.filter(
      (control) => !control.get('preCreatedActivity')?.value
    );
  }

  addAdditionalActivity() {
    this.workplanItems?.push(
      this.formBuilder.formGroup(new WorkplanActivityForm({}))
    );
  }

  removeAdditionalActivity(index: number) {
    this.workplanItems?.removeAt(index);
  }

  showPlannedStartDate(activityControl: AbstractControl<WorkplanActivityForm>) {
    const status = activityControl?.get('status')
      ?.value as WorkplanProgressType;
    return status === WorkplanProgressType.NotStarted;
  }

  showPlannedEndDate(activityControl: AbstractControl<WorkplanActivityForm>) {
    const status = activityControl?.get('status')
      ?.value as WorkplanProgressType;
    return (
      status === WorkplanProgressType.NotStarted ||
      status === WorkplanProgressType.InProgress
    );
  }

  showActualStartDate(activityControl: AbstractControl<WorkplanActivityForm>) {
    const status = activityControl?.get('status')
      ?.value as WorkplanProgressType;
    return (
      status === WorkplanProgressType.InProgress ||
      status === WorkplanProgressType.Completed
    );
  }

  showActualEndDate(activityControl: AbstractControl<WorkplanActivityForm>) {
    const status = activityControl?.get('status')
      ?.value as WorkplanProgressType;
    return status === WorkplanProgressType.Completed;
  }
}
