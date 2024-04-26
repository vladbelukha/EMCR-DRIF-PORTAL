import { Component, Input } from '@angular/core';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import {
  EOIApplicationForm,
  EngagementPlanForm,
} from '../eoi-application/eoi-application-form';
import { CommonModule } from '@angular/common';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { DrrTextareaComponent } from '../drr-datepicker/drr-textarea.component';

@Component({
  selector: 'drr-step-6',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    TranslocoModule,
    DrrTextareaComponent,
  ],
  templateUrl: './step-6.component.html',
  styleUrl: './step-6.component.scss',
})
export class Step6Component {
  @Input()
  engagementPlanForm!: IFormGroup<EngagementPlanForm>;

  getFormControl(name: string): FormControl {
    return this.engagementPlanForm.get(name) as FormControl;
  }
}
