import { Component, Input } from '@angular/core';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { EOIApplicationForm } from '../eoi-application/eoi-application-form';
import { CommonModule } from '@angular/common';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { DrrTextareaComponent } from '../drr-datepicker/drr-textarea.component';

@Component({
  selector: 'drr-step-7',
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
  templateUrl: './step-7.component.html',
  styleUrl: './step-7.component.scss',
})
export class Step7Component {
  @Input()
  eoiApplicationForm!: IFormGroup<EOIApplicationForm>;

  getFormControl(name: string): FormControl {
    return this.eoiApplicationForm.get(name) as FormControl;
  }
}
