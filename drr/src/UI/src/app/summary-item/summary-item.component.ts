import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import {
  RxFormArray,
  RxFormControl,
  RxFormGroup,
} from '@rxweb/reactive-form-validators';

@Component({
  selector: 'drr-summary-item',
  standalone: true,
  imports: [CommonModule, MatInputModule, TranslocoModule],
  templateUrl: './summary-item.component.html',
  styleUrl: './summary-item.component.scss',
})
export class SummaryItemComponent {
  @Input() label = '';

  private _formControl?: RxFormControl;
  @Input()
  set rxFormControl(rxFormControl: any) {
    this._formControl = rxFormControl as RxFormControl;
  }
  get rxFormControl() {
    return this._formControl;
  }

  isFormControl(): boolean {
    return this.rxFormControl instanceof RxFormControl;
  }

  isFormGroup(): boolean {
    return this.rxFormControl instanceof RxFormGroup;
  }

  isFormArray(): boolean {
    return this.rxFormControl instanceof RxFormArray;
  }

  objectKeys(obj: any) {
    return Object.keys(obj?.controls);
  }

  objectValues(obj: any) {
    return Object.values(obj?.controls);
  }

  objectHasValues(obj: any): boolean {
    // if array - check length, if value - check if truthy
    return (
      obj &&
      Object.values(obj).some((value) => {
        if (Array.isArray(value)) {
          return this.arrayHasValues(value);
        } else {
          return !!value;
        }
      })
    );
  }

  arrayHasValues(array: any[]): boolean {
    return (
      array &&
      array.length > 0 &&
      array.some((value) => this.objectHasValues(value))
    );
  }

  getGroupControl(controlName: string) {
    return this.rxFormControl?.get(controlName);
  }

  getGroupControlValue(controlName: string) {
    return this.rxFormControl?.get(controlName)?.value;
  }
}
