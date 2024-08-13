import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { FormArray, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { DateTime } from 'luxon';
import { DrrDatepickerComponent } from '../../../shared/controls/drr-datepicker/drr-datepicker.component';
import { DrrInputComponent } from '../../../shared/controls/drr-input/drr-input.component';
import { DrrSelectComponent } from '../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import { ProjectPlanForm, ProposedActivityForm } from '../drif-fp-form';

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
    TranslocoModule,
    DrrInputComponent,
    DrrSelectComponent,
    DrrDatepickerComponent,
    DrrTextareaComponent,
  ],
  templateUrl: './drif-fp-step-4.component.html',
  styleUrl: './drif-fp-step-4.component.scss',
})
export class DrifFpStep4Component {
  formBuilder = inject(RxFormBuilder);

  @Input() projectPlanForm!: IFormGroup<ProjectPlanForm>;

  minStartDate = new Date();
  minEndDate = new Date();

  ngOnInit() {
    this.projectPlanForm.get('startDate')?.valueChanges.subscribe((date) => {
      if (!DateTime.isDateTime(date)) {
        date = DateTime.fromISO(date.toString());
      }

      this.minEndDate = date.toJSDate();
    });
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
