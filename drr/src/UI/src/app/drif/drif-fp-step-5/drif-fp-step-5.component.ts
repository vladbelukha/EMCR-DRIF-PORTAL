import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { DrrInputComponent } from '../../shared/controls/drr-input/drr-input.component';
import { BudgetForm } from '../drif-fp/drif-fp-form';

@Component({
  selector: 'drif-fp-step-5',
  standalone: true,
  imports: [
    CommonModule,
    MatInputModule,
    DrrInputComponent,
    TranslocoModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  templateUrl: './drif-fp-step-5.component.html',
  styleUrl: './drif-fp-step-5.component.scss',
})
export class DrifFpStep5Component {
  @Input()
  budgetForm!: IFormGroup<BudgetForm>;
}
