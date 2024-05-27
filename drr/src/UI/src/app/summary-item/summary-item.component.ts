import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { Component, Input, inject } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import {
  RxFormArray,
  RxFormControl,
  RxFormGroup,
} from '@rxweb/reactive-form-validators';

@Component({
  selector: 'drr-summary-item',
  standalone: true,
  imports: [CommonModule, MatInputModule, TranslocoModule],
  providers: [DatePipe, CurrencyPipe],
  templateUrl: './summary-item.component.html',
  styleUrl: './summary-item.component.scss',
})
export class SummaryItemComponent {
  translocoService = inject(TranslocoService);
  datePipe = inject(DatePipe);
  currencyPipe = inject(CurrencyPipe);

  @Input() key = '';

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

  getControlValue() {
    if (this.key === 'relatedHazards') {
      return this.rxFormControl?.value
        ?.map((hazard: string) => this.translocoService.translate(hazard))
        .join(', ');
    }

    const booleanKeys = ['ownershipDeclaration'];
    if (booleanKeys.includes(this.key)) {
      return this.rxFormControl?.value === null
        ? this.rxFormControl?.value
        : this.rxFormControl?.value
        ? 'Yes'
        : 'No';
    }

    const translateKeys = ['proponentType', 'fundingStream', 'projectType'];
    if (translateKeys.includes(this.key)) {
      return this.translocoService.translate(this.rxFormControl?.value);
    }

    const arrayKeys = ['partneringProponents', 'infrastructureImpacted'];
    if (arrayKeys.includes(this.key)) {
      return this.rxFormControl?.value?.join(', ');
    }

    const dateKeys = ['startDate', 'endDate'];
    if (dateKeys.includes(this.key)) {
      return this.datePipe.transform(this.rxFormControl?.value, 'yyyy-MM-dd');
    }

    if (this.key === 'remainingAmount') {
      return this.currencyPipe.transform(Math.abs(this.rxFormControl?.value));
    }

    return this.rxFormControl?.value;
  }

  getGroupControlValue(controlName: string) {
    return this.rxFormControl?.get(controlName)?.value;
  }

  extractArrayControlValue(controlName: string, control: RxFormControl) {
    const translateKeys = ['type'];
    if (translateKeys.includes(controlName)) {
      return this.translocoService.translate(control.get(controlName)?.value);
    }

    return control.get(controlName)?.value;
  }

  showConditionalControl(controlName: string, control: RxFormControl) {
    if (controlName === 'otherDescription') {
      return (
        control?.get(controlName)?.hasError('required') ||
        control?.get(controlName)?.value
      );
    }

    if (controlName === 'otherHazardsDescription') {
      return control.hasError('required');
    }

    return true;
  }
}
