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
  DeclarationType,
  Delay,
  DocumentType,
  FormType,
  InterimProjectType,
  ProgressReport,
  ProjectProgressStatus,
  RecordType,
  SignageType,
  WorkplanStatus,
  YesNoOption,
} from '../../../../../model';

import { AbstractControl, FormArray, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { ActivatedRoute, Router } from '@angular/router';
import { HotToastService } from '@ngxpert/hot-toast';
import { Subscription } from 'rxjs';
import { AttachmentService } from '../../../../../api/attachment/attachment.service';
import { ProjectService } from '../../../../../api/project/project.service';
import { DrrDatepickerComponent } from '../../../../shared/controls/drr-datepicker/drr-datepicker.component';
import { DrrFileUploadComponent } from '../../../../shared/controls/drr-file-upload/drr-file-upload.component';
import { DrrInputComponent } from '../../../../shared/controls/drr-input/drr-input.component';
import { DrrNumericInputComponent } from '../../../../shared/controls/drr-number-input/drr-number-input.component';
import {
  DrrRadioButtonComponent,
  RadioOption,
} from '../../../../shared/controls/drr-radio-button/drr-radio-button.component';
import {
  DrrSelectComponent,
  DrrSelectOption,
} from '../../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../../shared/controls/drr-textarea/drr-textarea.component';
import { FileService } from '../../../../shared/services/file.service';
import { OptionsStore } from '../../../../store/options.store';
import { AttachmentForm } from '../../../drif-fp/drif-fp-form';
import { DrrAttahcmentComponent } from '../../../drif-fp/drif-fp-step-11/drif-fp-attachment.component';
import {
  EventInformationForm,
  EventProgressType,
  FundingSignageForm,
  ProgressReportForm,
  ProjectEventForm,
  WorkplanActivityForm,
  WorkplanForm,
} from '../drif-progress-report-form';
import { DrifProgressReportSummaryComponent } from '../drif-progress-report-summary/drif-progress-report-summary.component';

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
    MatCheckboxModule,
    TranslocoModule,
    DrrDatepickerComponent,
    DrrInputComponent,
    DrrNumericInputComponent,
    DrrSelectComponent,
    DrrRadioButtonComponent,
    DrrTextareaComponent,
    DrrAttahcmentComponent,
    DrrFileUploadComponent,
    RxReactiveFormsModule,
    MatDividerModule,
    DrifProgressReportSummaryComponent,
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
  toastService = inject(HotToastService);
  attachmentsService = inject(AttachmentService);
  fileService = inject(FileService);
  optionsStore = inject(OptionsStore);

  projectId!: string;
  reportId!: string;
  progressReportId!: string;

  stepperOrientation: StepperOrientation = 'horizontal';

  progressReportForm = this.formBuilder.formGroup(
    ProgressReportForm,
    {},
  ) as IFormGroup<ProgressReportForm>;

  authorizedRepresentativeText?: string;
  accuracyOfInformationText?: string;

  private allActivityTypeOptions: DrrSelectOption[] = Object.values(
    ActivityType,
  ).map((activity) => ({
    value: activity,
    label: this.translocoService.translate(`activityType.${activity}`),
  }));

  optionalActivityStatusOptions: DrrSelectOption[] = Object.values(
    WorkplanStatus,
  )
    .filter(
      (s) => s !== WorkplanStatus.NotAwarded && s !== WorkplanStatus.Awarded,
    )
    .map((value) => ({
      label: this.translocoService.translate(`workplanStatus.${value}`),
      value,
    }));

  necessaryActivityStatusOptions: RadioOption[] =
    this.optionalActivityStatusOptions.filter(
      (option) => option.value !== WorkplanStatus.NoLongerNeeded,
    );

  milestoneStatusOptions: DrrSelectOption[] = Object.values(WorkplanStatus)
    .filter(
      (s) => s === WorkplanStatus.NotAwarded || s === WorkplanStatus.Awarded,
    )
    .map((value) => ({
      label: this.translocoService.translate(`workplanStatus.${value}`),
      value,
    }));

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
    }),
  );

  projectProgressOptions = Object.keys(ProjectProgressStatus).map((key) => ({
    label: this.translocoService.translate(`projectProgress.${key}`),
    value: key,
  }));

  delayReasonOptions: DrrSelectOption[] = Object.values(Delay).map((value) => ({
    label: this.translocoService.translate(`delayReason.${value}`),
    value,
  }));

  signageTypeOptions: DrrSelectOption[] = Object.values(SignageType).map(
    (value) => ({
      label: this.translocoService.translate(`signageType.${value}`),
      value,
    }),
  );

  get workplanForm(): IFormGroup<WorkplanForm> | null {
    return this.progressReportForm.get('workplan') as IFormGroup<WorkplanForm>;
  }

  get workplanItems(): FormArray | null {
    return this.workplanForm?.get('workplanActivities') as FormArray;
  }

  get eventsForm(): IFormGroup<EventInformationForm> | null {
    return this.progressReportForm.get(
      'eventInformation',
    ) as IFormGroup<EventInformationForm>;
  }

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = params['projectId'];
      this.reportId = params['reportId'];
      this.progressReportId = params['progressReportId'];

      this.authorizedRepresentativeText = this.optionsStore.getDeclarations?.(
        DeclarationType.AuthorizedRepresentative,
        FormType.Report,
      );

      this.accuracyOfInformationText = this.optionsStore.getDeclarations?.(
        DeclarationType.AccuracyOfInformation,
        FormType.Report,
      );

      this.projectService
        .projectGetProgressReport(
          this.projectId,
          this.reportId,
          this.progressReportId,
        )
        .subscribe((report: ProgressReport) => {
          report.workplan?.workplanActivities?.map((activity) => {
            const activityForm = this.formBuilder.formGroup(
              new WorkplanActivityForm(activity),
            );

            this.workplanItems?.push(activityForm);
          });

          if (
            report?.workplan?.fundingSignage
              ? report?.workplan?.fundingSignage.length > 0
              : false
          ) {
            this.getSignageFormArray().clear();
          }
          report.workplan?.fundingSignage?.map((signage) => {
            const signageForm = this.formBuilder.formGroup(
              new FundingSignageForm(signage),
            );

            this.getSignageFormArray()?.push(signageForm);
          });

          this.progressReportForm
            .get('workplan.fundingSourcesChanged')
            ?.valueChanges.subscribe((value) => {
              const comment = this.progressReportForm.get(
                'workplan.fundingSourcesChangedComment',
              );

              if (value) {
                comment?.addValidators(Validators.required);
              } else {
                comment?.removeValidators(Validators.required);
              }

              comment?.updateValueAndValidity();
            });

          this.progressReportForm
            .get('workplan.outstandingIssues')
            ?.valueChanges.subscribe((value) => {
              const comment = this.progressReportForm.get(
                'workplan.outstandingIssuesComments',
              );

              if (value) {
                comment?.addValidators(Validators.required);
              } else {
                comment?.removeValidators(Validators.required);
              }

              comment?.updateValueAndValidity();
            });

          this.progressReportForm
            .get('workplan.mediaAnnouncement')
            ?.valueChanges.subscribe((value) => {
              const date = this.progressReportForm.get(
                'workplan.mediaAnnouncementDate',
              );
              const comment = this.progressReportForm.get(
                'workplan.mediaAnnouncementComment',
              );

              if (value) {
                date?.addValidators(Validators.required);
                comment?.addValidators(Validators.required);
              } else {
                date?.removeValidators(Validators.required);
                comment?.removeValidators(Validators.required);
              }

              date?.updateValueAndValidity();
              comment?.updateValueAndValidity();
            });

          report.eventInformation?.pastEvents?.map((event) => {
            this.getPastEventsArray()?.push(
              this.formBuilder.formGroup(new ProjectEventForm(event)),
            );
          });

          report.eventInformation?.upcomingEvents?.map((event) => {
            this.getUpcomingEventsArray()?.push(
              this.formBuilder.formGroup(new ProjectEventForm(event)),
            );
          });

          this.eventsForm
            ?.get('eventsOccurredSinceLastReport')
            ?.valueChanges.subscribe((value) => {
              if (value === true && this.getPastEventsArray()?.length === 0) {
                this.addPastEvent();
              }
              if (value === false) {
                this.getPastEventsArray()?.clear();
              }
            });

          this.eventsForm
            ?.get('anyUpcomingEvents')
            ?.valueChanges.subscribe((value) => {
              if (
                value === true &&
                this.getUpcomingEventsArray()?.length === 0
              ) {
                this.addFutureEvent();
              }
              if (value === false) {
                this.getUpcomingEventsArray()?.clear();
              }
            });

          report.attachments?.map((attachment) => {
            const attachmentForm = this.formBuilder.formGroup(
              new AttachmentForm(attachment),
            );

            this.getAttachmentsFormArray().push(attachmentForm);
          });

          this.progressReportForm.patchValue(report);
        });

      this.progressReportForm
        .get('workplan.projectProgress')
        ?.valueChanges.subscribe((value) => {
          const delayReason = this.progressReportForm.get(
            'workplan.delayReason',
          );
          const otherDelayReason = this.progressReportForm.get(
            'workplan.otherDelayReason',
          );
          const behindScheduleMitigatingComments = this.progressReportForm.get(
            'workplan.behindScheduleMitigatingComments',
          );
          const aheadOfScheduleComments = this.progressReportForm.get(
            'workplan.aheadOfScheduleComments',
          );

          let delayReasonSub: Subscription | undefined;

          if (value === ProjectProgressStatus.BehindSchedule) {
            delayReason?.addValidators(Validators.required);
            delayReasonSub = delayReason?.valueChanges.subscribe((reason) => {
              if (reason === Delay.Other) {
                otherDelayReason?.addValidators(Validators.required);
              } else {
                otherDelayReason?.removeValidators(Validators.required);
                otherDelayReason?.reset();
              }
            });
            behindScheduleMitigatingComments?.addValidators(
              Validators.required,
            );
          } else {
            delayReason?.removeValidators(Validators.required);
            delayReason?.reset();
            delayReasonSub?.unsubscribe();
            behindScheduleMitigatingComments?.removeValidators(
              Validators.required,
            );
            behindScheduleMitigatingComments?.reset();
          }

          if (value === ProjectProgressStatus.AheadOfSchedule) {
            aheadOfScheduleComments?.addValidators(Validators.required);
          } else {
            aheadOfScheduleComments?.removeValidators(Validators.required);
            aheadOfScheduleComments?.reset();
          }

          delayReason?.updateValueAndValidity();
          otherDelayReason?.updateValueAndValidity();
          behindScheduleMitigatingComments?.updateValueAndValidity();
          aheadOfScheduleComments?.updateValueAndValidity();
        });
    });
  }

  stepperSelectionChange(event: any) {}

  save() {
    this.projectService
      .projectUpdateProgressReport(
        this.projectId,
        this.reportId,
        this.progressReportId,
        this.progressReportForm.getRawValue(),
      )
      .subscribe(() => {
        this.toastService.success('Progress report saved');
      });
  }

  goBack() {
    // TODO: save

    this.router.navigate(['drif-projects', this.projectId]);
  }

  submit() {}

  getActivitiesFormArray() {
    return this.workplanForm?.get('workplanActivities') as FormArray;
  }

  getPreDefinedActivitiesArray() {
    return this.workplanItems?.controls.filter(
      (control) =>
        control.get('preCreatedActivity')?.value &&
        control.get('activity')?.value !== ActivityType.PermitToConstruct &&
        control.get('activity')?.value !==
          ActivityType.ConstructionContractAward,
    );
  }

  getMilestoneActivitiesArray() {
    return this.workplanItems?.controls.filter(
      (control) =>
        control.get('preCreatedActivity')?.value &&
        (control.get('activity')?.value === ActivityType.PermitToConstruct ||
          control.get('activity')?.value ===
            ActivityType.ConstructionContractAward),
    );
  }

  getPreDefinedActivityStatusOptions(preDefinedActivity: ActivityType) {
    if (
      preDefinedActivity === ActivityType.ConstructionContractAward ||
      preDefinedActivity === ActivityType.PermitToConstruct
    ) {
      return this.milestoneStatusOptions;
    }

    return this.necessaryActivityStatusOptions;
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
        }),
      ),
    );
  }

  getAdditionalActivityOptions(activityControl: AbstractControl) {
    if (!this.isAdditionalActivityMandatory(activityControl)) {
      return this.necessaryActivityStatusOptions;
    }

    return this.optionalActivityStatusOptions;
  }

  isAdditionalActivityMandatory(activityControl: AbstractControl) {
    return !!activityControl.get('isMandatory')?.value;
  }

  removeAdditionalActivity(index: number) {
    this.workplanItems?.removeAt(index);
  }

  showPlannedStartDate(activityControl: AbstractControl<WorkplanActivityForm>) {
    const status = activityControl?.get('status')?.value as WorkplanStatus;
    return (
      status === WorkplanStatus.NotStarted ||
      status === WorkplanStatus.NotAwarded
    );
  }

  showPlannedCompletionDate(
    activityControl: AbstractControl<WorkplanActivityForm>,
  ) {
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
      status === WorkplanStatus.Completed ||
      status === WorkplanStatus.Awarded
    );
  }

  showActualCompletionDate(
    activityControl: AbstractControl<WorkplanActivityForm>,
  ) {
    const status = activityControl?.get('status')?.value as WorkplanStatus;
    return status === WorkplanStatus.Completed;
  }

  getAvailableOptionsForActivity(selectedActivity: ActivityType) {
    const selectedActivities = this.getActivitiesFormArray()?.controls.map(
      (control) => control.get('activity')?.value,
    );

    const availableOptions = this.allActivityTypeOptions.filter(
      (option) => !selectedActivities.includes(option.value),
    );

    if (selectedActivity) {
      const selectedActivityOption = this.allActivityTypeOptions.find(
        (option) => option.value === selectedActivity,
      );

      availableOptions.push(selectedActivityOption!);
      availableOptions.sort((a, b) => a.label.localeCompare(b.label));
    }

    return availableOptions;
  }

  hasAvailableOptionsForActivity() {
    const selectedActivities = this.getActivitiesFormArray()?.controls.map(
      (control) => control.get('activity')?.value,
    );

    return this.allActivityTypeOptions.length !== selectedActivities.length;
  }

  isProjectDelayed() {
    return (
      this.workplanForm?.get('projectProgress')?.value ===
      ProjectProgressStatus.BehindSchedule
    );
  }

  isProjectAhead() {
    return (
      this.workplanForm?.get('projectProgress')?.value ===
      ProjectProgressStatus.AheadOfSchedule
    );
  }

  isOtherDelayReasonSelected() {
    return this.workplanForm?.get('delayReason')?.value === Delay.Other;
  }

  isStructuralProject() {
    return (
      this.progressReportForm.get('projectType')?.value ===
      InterimProjectType.Stream2
    );
  }

  getSignageFormArray() {
    return this.workplanForm?.get('fundingSignage') as FormArray;
  }

  addSignage() {
    this.getSignageFormArray()?.push(
      this.formBuilder.formGroup(new FundingSignageForm({})),
    );
  }

  removeSignage(id: string) {
    const index = this.getSignageFormArray()?.controls.findIndex(
      (control) => control.get('id')?.value === id,
    );

    this.getSignageFormArray()?.removeAt(index!);
  }

  getPastEventsArray() {
    return this.eventsForm?.get('pastEvents') as FormArray;
  }

  addPastEvent() {
    this.getPastEventsArray()?.push(
      this.formBuilder.formGroup(new ProjectEventForm({})),
    );
  }

  removePastEvent(index: number) {
    this.getPastEventsArray()?.removeAt(index);
  }

  getUpcomingEventsArray() {
    return this.eventsForm?.get('upcomingEvents') as FormArray;
  }

  addFutureEvent() {
    this.getUpcomingEventsArray()?.push(
      this.formBuilder.formGroup(new ProjectEventForm({})),
    );
  }

  removeFutureEvent(index: number) {
    this.getUpcomingEventsArray()?.removeAt(index);
  }

  getAttachmentsFormArray(): FormArray {
    return this.progressReportForm.get('attachments') as FormArray;
  }

  async uploadFiles(files: File[]) {
    files.forEach(async (file) => {
      if (file == null) {
        return;
      }

      const base64Content = await this.fileService.fileToBase64(file);

      this.attachmentsService
        .attachmentUploadAttachment({
          recordId: this.progressReportId,
          recordType: RecordType.ProgressReport,
          documentType: DocumentType.ProgressReport,
          name: file.name,
          contentType:
            file.type === ''
              ? this.fileService.getCustomContentType(file)
              : file.type,
          content: base64Content.split(',')[1],
        })
        .subscribe({
          next: (attachment) => {
            const attachmentFormData = {
              name: file.name,
              comments: '',
              id: attachment.id,
              documentType: DocumentType.ProgressReport,
            } as AttachmentForm;

            this.getAttachmentsFormArray().push(
              this.formBuilder.formGroup(AttachmentForm, attachmentFormData),
            );
          },
          error: () => {
            this.toastService.close();
            this.toastService.error('File upload failed');
          },
        });
    });
  }

  downloadFile(fileId: string) {
    this.fileService.downloadFile(fileId);
  }

  removeFile(fileId: string) {
    this.attachmentsService
      .attachmentDeleteAttachment(fileId, {
        recordId: this.progressReportId,
        id: fileId,
      })
      .subscribe({
        next: () => {
          const attachmentsArray = this.progressReportForm.get(
            'attachments',
          ) as FormArray;
          const fileIndex = attachmentsArray.controls.findIndex(
            (control) => control.value.id === fileId,
          );

          const documentType = attachmentsArray.controls[fileIndex].value
            .documentType as DocumentType;

          attachmentsArray.removeAt(fileIndex);
        },
        error: () => {
          this.toastService.close();
          this.toastService.error('File deletion failed');
        },
      });
  }

  getDelcarationForm() {
    return this.progressReportForm.get('declaration') as IFormGroup<any>;
  }
}
