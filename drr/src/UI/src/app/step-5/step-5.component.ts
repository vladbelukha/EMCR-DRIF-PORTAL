import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { EOIApplicationForm } from '../eoi-application/eoi-application-form';

@Component({
  selector: 'drr-step-5',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './step-5.component.html',
  styleUrl: './step-5.component.scss',
})
export class Step5Component {
  @Input()
  eoiApplicationForm!: IFormGroup<EOIApplicationForm>;
}
