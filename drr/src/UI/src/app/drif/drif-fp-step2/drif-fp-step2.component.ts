import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { DrrTextareaComponent } from '../../shared/controls/drr-textarea/drr-textarea.component';
import { ProponentEligibilityForm } from '../drif-fp/drif-fp-form';

@Component({
  selector: 'drif-fp-step-2',
  standalone: true,
  imports: [
    CommonModule,
    TranslocoModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatRadioModule,
    DrrTextareaComponent,
  ],
  templateUrl: './drif-fp-step2.component.html',
  styleUrl: './drif-fp-step2.component.scss',
})
export class DrifFpStep2Component {
  @Input()
  proponentEligibilityForm!: IFormGroup<ProponentEligibilityForm>;
}
