import { Component, Input, inject } from '@angular/core';
import { EOIApplicationForm } from '../eoi-application/eoi-application-form';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { Step1Component } from '../step-1/step-1.component';
import { SummaryComponent } from '../summary/summary.component';

@Component({
  selector: 'drr-step-8',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    SummaryComponent,
  ],
  templateUrl: './step-8.component.html',
  styleUrl: './step-8.component.scss',
})
export class Step8Component {
  @Input()
  eoiApplicationForm!: IFormGroup<EOIApplicationForm>;
}
