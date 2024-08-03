import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { AsyncPipe, CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatAutocompleteModule,
  MatAutocompleteSelectedEvent,
} from '@angular/material/autocomplete';
import { MatChipInputEvent, MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { TranslocoModule } from '@ngneat/transloco';
import { RxFormBuilder } from '@rxweb/reactive-form-validators';
import { map, Observable, startWith } from 'rxjs';
import { Standards } from '../drif-fp/drif-fp-form';

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
  ],
  templateUrl: './drif-fp-step-3.component.html',
  styleUrl: './drif-fp-step-3.component.scss',
})
export class DrifFpStep3Component {
  formBuilder = inject(RxFormBuilder);

  separatorKeysCodes: number[] = [ENTER, COMMA];
  availableStandards: string[] = Object.values(Standards);
  selectedStandards = signal<string[]>([]);
  // TODO: make this an @Input?
  standardControl = new FormControl('');
  filteredStandards?: Observable<string[]>;

  ngOnInit() {
    this.filteredStandards = this.standardControl.valueChanges.pipe(
      startWith(null),
      map((standard: string | null) =>
        standard ? this._filter(standard) : this.availableStandards.slice()
      )
    );
  }

  addStandard(event: MatChipInputEvent) {
    const value = (event.value || '').trim();

    if (!value || this.selectedStandards().includes(value)) {
      return;
    }

    this.selectedStandards.update((standards) => [...standards, value]);

    event.chipInput!.clear();
    this.standardControl.setValue(null);
  }

  removeStandard(index: number) {
    this.selectedStandards.update((standards) => {
      standards.splice(index, 1);
      return standards;
    });
  }

  standardSelected(event: MatAutocompleteSelectedEvent) {
    this.selectedStandards.update((standards) => [
      ...standards,
      event.option.viewValue,
    ]);
    this.standardControl.setValue('');
    event.option.deselect();
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.availableStandards.filter((standard) =>
      standard.toLowerCase().includes(filterValue)
    );
  }
}
