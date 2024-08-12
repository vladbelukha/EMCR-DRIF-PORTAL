import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { AsyncPipe, CommonModule } from '@angular/common';
import { Component, inject, Input, signal } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatAutocompleteModule,
  MatAutocompleteSelectedEvent,
} from '@angular/material/autocomplete';
import { MatChipInputEvent, MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { TranslocoModule } from '@ngneat/transloco';
import { RxFormBuilder, RxFormControl } from '@rxweb/reactive-form-validators';
import { map, Observable, startWith } from 'rxjs';

@Component({
  selector: 'drr-chip-autocomplete',
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
  templateUrl: './drr-chip-autocomplete.component.html',
  styleUrl: './drr-chip-autocomplete.component.scss',
})
export class DrrChipAutocompleteComponent {
  formBuilder = inject(RxFormBuilder);

  @Input()
  label = '';

  @Input()
  placeholder = '';

  @Input()
  options?: string[];

  private _formControl = this.formBuilder.control('', []) as RxFormControl;
  @Input()
  set rxFormControl(rxFormControl: any) {
    this._formControl = rxFormControl as RxFormControl;
  }
  get rxFormControl() {
    return this._formControl;
  }

  separatorKeysCodes: number[] = [ENTER, COMMA];
  inputControl = new FormControl('');

  selectedOptions = signal<string[]>([]);
  filteredOptions?: Observable<string[]>;

  ngOnInit() {
    this.rxFormControl.statusChanges.subscribe((status: any) => {
      if (status === 'DISABLED') {
        this.selectedOptions.update(() => []);
        this.rxFormControl.setValue([], { emitEvent: false });
      }
    });

    if (this.rxFormControl.value?.length) {
      this.selectedOptions.update((standards) => [
        ...standards,
        ...this.rxFormControl.value,
      ]);
    }

    this.filteredOptions = this.inputControl.valueChanges.pipe(
      startWith(null),
      map((standard: string | null) =>
        standard ? this._filter(standard) : this.options!.slice()
      )
    );
  }

  addOption(event: MatChipInputEvent) {
    const value = (event.value || '').trim();

    if (!value || this.selectedOptions().includes(value)) {
      return;
    }

    this.selectedOptions.update((standards) => [...standards, value]);

    event.chipInput!.clear();
    this.inputControl.setValue(null);
  }

  removeOption(index: number) {
    this.selectedOptions.update((standards) => {
      standards.splice(index, 1);
      return standards;
    });
  }

  optionSelected(event: MatAutocompleteSelectedEvent) {
    this.selectedOptions.update((standards) => [
      ...standards,
      event.option.viewValue,
    ]);
    this.inputControl.setValue('');
    event.option.deselect();
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.options!.filter((standard) =>
      standard.toLowerCase().includes(filterValue)
    );
  }
}
