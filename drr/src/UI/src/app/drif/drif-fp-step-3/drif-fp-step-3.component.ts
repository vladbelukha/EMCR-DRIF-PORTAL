import { AsyncPipe, CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { DrrChipAutocompleteComponent } from '../../shared/controls/drr-chip-autocomplete/drr-chip-autocomplete.component';
import {
  PermitsRegulationsAndStandardsForm,
  Standards,
} from '../drif-fp/drif-fp-form';

@Component({
  selector: 'drif-fp-step-3',
  standalone: true,
  imports: [
    CommonModule,
    MatChipsModule,
    MatIconModule,
    TranslocoModule,
    MatFormFieldModule,
    FormsModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    AsyncPipe,
    DrrChipAutocompleteComponent,
  ],
  templateUrl: './drif-fp-step-3.component.html',
  styleUrl: './drif-fp-step-3.component.scss',
})
export class DrifFpStep3Component {
  @Input()
  permitsRegulationsAndStandardsForm!: IFormGroup<PermitsRegulationsAndStandardsForm>;

  provincialStandardsOptions = Object.values(Standards);

  ngOnInit() {}
}
