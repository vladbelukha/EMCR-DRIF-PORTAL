import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { FormArray, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { v4 as uuidv4 } from 'uuid';
import { ActivityType, FundingStream } from '../../../../model';
import { DrrChipAutocompleteComponent } from '../../../shared/controls/drr-chip-autocomplete/drr-chip-autocomplete.component';
import { DrrDatepickerComponent } from '../../../shared/controls/drr-datepicker/drr-datepicker.component';
import { DrrInputComponent } from '../../../shared/controls/drr-input/drr-input.component';
import {
  DrrSelectComponent,
  DrrSelectOption,
} from '../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import { OptionsStore } from '../../../store/options.store';
import { ProjectPlanForm, ProposedActivityForm } from '../drif-fp-form';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drif-fp-step-4',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    TranslocoModule,
    DrrInputComponent,
    DrrSelectComponent,
    DrrDatepickerComponent,
    DrrTextareaComponent,
    DrrChipAutocompleteComponent,
  ],
  templateUrl: './drif-fp-step-4.component.html',
  styleUrl: './drif-fp-step-4.component.scss',
})
export class DrifFpStep4Component {
  formBuilder = inject(RxFormBuilder);
  optionsStore = inject(OptionsStore);
  translocoService = inject(TranslocoService);

  private commonActivityOptions: DrrSelectOption[] = Object.values(ActivityType)
    .filter(
      (activity) =>
        activity === ActivityType.Administration ||
        activity === ActivityType.ProjectPlanning ||
        activity === ActivityType.Assessment ||
        activity === ActivityType.Mapping ||
        activity === ActivityType.LandAcquisition ||
        activity === ActivityType.ApprovalsPermitting ||
        activity === ActivityType.Communications ||
        activity === ActivityType.AffectedPartiesEngagement ||
        activity === ActivityType.CommunityEngagement,
    )
    .map((activity) => ({
      value: activity,
      label: this.translocoService.translate(`activityType.${activity}`),
    }));

  private nonStructuralActivities: ActivityType[] = [
    ActivityType.Project,
    ActivityType.FirstNationsEngagement,
  ];
  private nonStructuralActivityOptions: DrrSelectOption[] = [
    ...this.commonActivityOptions,
    ...this.nonStructuralActivities.map((activity) => ({
      value: activity,
      label: this.translocoService.translate(`activityType.${activity}`),
    })),
  ];

  private structuralActivities: ActivityType[] = [
    ActivityType.Project,
    ActivityType.FirstNationsEngagement,
    ActivityType.Design,
    ActivityType.ConstructionTender,
    ActivityType.Construction,
    ActivityType.ConstructionContractAward,
    ActivityType.PermitToConstruct,
  ];
  private structuralActivityOptions: DrrSelectOption[] = [
    ...this.commonActivityOptions,
    ...this.structuralActivities.map((activity) => ({
      value: activity,
      label: this.translocoService.translate(`activityType.${activity}`),
    })),
  ];

  @Input() projectPlanForm!: IFormGroup<ProjectPlanForm>;
  @Input() fundingStream?: string;

  minStartDate = new Date();

  verificationMethodOptions = this.optionsStore
    .getOptions()
    ?.foundationalOrPreviousWorks?.();

  getNextDayAfterStartDate() {
    const startDate = this.projectPlanForm.get('startDate')?.value;
    if (startDate) {
      const nextDay = new Date(startDate);
      nextDay.setDate(nextDay.getDate() + 1);
      return nextDay;
    }
    return this.minStartDate;
  }

  getPreDefinedActivitiesArray() {
    return this.getActivitiesFormArray()?.controls.filter(
      (control) => control.get('preCreatedActivity')?.value,
    );
  }

  getAdditionalActivitiesArray() {
    return this.getActivitiesFormArray()?.controls.filter(
      (control) => !control.get('preCreatedActivity')?.value,
    );
  }

  getActivitiesFormArray() {
    return this.projectPlanForm.get('proposedActivities') as FormArray;
  }

  addActivity() {
    const newActivity = this.formBuilder.formGroup(ProposedActivityForm);

    newActivity.get('id')?.setValue(uuidv4());

    this.getActivitiesFormArray().push(newActivity);
  }

  removeActivity(id: string) {
    this.getActivitiesFormArray().removeAt(
      this.getActivitiesFormArray().controls.findIndex(
        (control) => control.get('id')?.value === id,
      ),
    );
  }

  getAvailableOptionsForActivity(selectedActivity: ActivityType) {
    const selectedActivities = this.getActivitiesFormArray()?.controls.map(
      (control) => control.get('activity')?.value,
    );

    const availableOptions = this.getAvailableOptionsFundingStream().filter(
      (option) => !selectedActivities.includes(option.value),
    );

    if (selectedActivity) {
      const selectedActivityOption =
        this.getAvailableOptionsFundingStream().find(
          (option) => option.value === selectedActivity,
        );

      availableOptions.push(selectedActivityOption!);
      availableOptions.sort((a, b) => a.label.localeCompare(b.label));
    }

    return availableOptions;
  }

  showStartDate(activityType: ActivityType) {
    return (
      activityType !== ActivityType.ConstructionContractAward &&
      activityType !== ActivityType.PermitToConstruct
    );
  }

  private getAvailableOptionsFundingStream() {
    return this.fundingStream === FundingStream.Stream2
      ? this.structuralActivityOptions
      : this.nonStructuralActivityOptions;
  }
}
