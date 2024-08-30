import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import { RxFormControl } from '@rxweb/reactive-form-validators';

export type ControlType =
  | 'input'
  | 'textarea'
  | 'select'
  | 'radio'
  | 'checkbox';

@Component({
  selector: 'drr-summary-item',
  standalone: true,
  imports: [CommonModule, MatInputModule, TranslocoModule],
  templateUrl: './summary-item.component.html',
  styleUrl: './summary-item.component.scss',
})
export class SummaryItemComponent {
  translocoService = inject(TranslocoService);

  @Input() label?: string;

  private _value: string | null | undefined;
  @Input()
  set value(value: string | null | undefined) {
    this._value = value;
  }
  get value(): string | null | undefined {
    if (this._value !== undefined) {
      return this._value;
    }

    const controlValue = this.rxFormControl?.value;

    switch (this.controlType) {
      case 'input':
      case 'textarea':
        return this.rxFormControl?.value;
      case 'radio':
        return typeof controlValue === 'boolean'
          ? this.translocoService.translate(controlValue ? 'Yes' : 'No')
          : this.translocoService.translate(controlValue);

      default:
        return this.rxFormControl?.value;
    }
  }

  @Input() controlType: ControlType = 'input';

  @Input() rxFormControl?: RxFormControl;

  isRequired(): boolean {
    if (this.rxFormControl && this.rxFormControl.validator) {
      const validator = this.rxFormControl.validator({} as AbstractControl);
      return validator && validator['required'];
    }
    return false;
  }
}
