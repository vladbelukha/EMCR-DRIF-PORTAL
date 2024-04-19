import { Component, Input } from '@angular/core';
import { EOIApplicationForm } from '../eoi-application/eoi-application-form';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { TranslocoModule } from '@ngneat/transloco';

@Component({
  selector: 'drr-step-4',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatRadioModule,
    TranslocoModule,
  ],
  templateUrl: './step-4.component.html',
  styleUrl: './step-4.component.scss',
})
export class Step4Component {
  @Input()
  eoiApplicationForm!: IFormGroup<EOIApplicationForm>;

  locationDescriptionMax = 500;

  // TODO: make a textarea component
  getCount() {
    return this.eoiApplicationForm.value.locationDescription?.length ?? 0;
  }
}
