import { Component, Input } from '@angular/core';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { EOIApplicationForm } from '../eoi-application/eoi-application-form';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';

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
  ],
  templateUrl: './step-6.component.html',
  styleUrl: './step-6.component.scss',
})
export class Step6Component {
  @Input()
  eoiApplicationForm!: IFormGroup<EOIApplicationForm>;
}
