import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { FormArray, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
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

  activityOptions: DrrSelectOption[] = [
    {
      label: 'Test Activity 1',
      value: 'Test Activity 1',
    },
    {
      label: 'Test Activity 2',
      value: 'Test Activity 2',
    },
  ]; // TODO: this.optionsStore.getOptions()?.activities?.();

  @Input() projectPlanForm!: IFormGroup<ProjectPlanForm>;

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

  getActivitiesFormArray() {
    return this.projectPlanForm.get('proposedActivities') as FormArray;
  }

  addActivity() {
    this.getActivitiesFormArray().push(
      this.formBuilder.formGroup(ProposedActivityForm)
    );
  }

  removeActivity(index: number) {
    this.getActivitiesFormArray().removeAt(index);
  }
}
