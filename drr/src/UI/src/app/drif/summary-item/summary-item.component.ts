import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
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
  | 'checkbox'
  | 'array'
  | 'date'
  | 'currency';

@Component({
  selector: 'drr-summary-item',
  standalone: true,
  imports: [CommonModule, MatInputModule, TranslocoModule],
  templateUrl: './summary-item.component.html',
  styleUrl: './summary-item.component.scss',
  providers: [DatePipe, CurrencyPipe],
})
export class SummaryItemComponent {
  translocoService = inject(TranslocoService);
  datePipe = inject(DatePipe);
  currencyPipe = inject(CurrencyPipe);

  @Input() label?: string;

  private _value: string | number | null | undefined;
  @Input()
  set value(value: string | number | null | undefined) {
    this._value = value;
  }
  get value(): string | number | null | undefined {
    if (this._value !== undefined) {
      return this._value;
    }

    const controlValue = this.rxFormControl?.value;

    switch (this.controlType) {
      case 'input':
      case 'textarea':
        return this.rxFormControl?.value;
      case 'select':
        return this.translocoService.translate(controlValue);
      case 'radio':
        return typeof controlValue === 'boolean'
          ? this.translocoService.translate(controlValue ? 'Yes' : 'No')
          : this.translocoService.translate(controlValue);
      case 'array':
        return (
          this.translate
            ? (this.translocoService.translate(
                this.rxFormControl?.value
              ) as string[])
            : (this.rxFormControl?.value as string[])
        )?.join(', ');
      case 'date':
        return this.datePipe.transform(controlValue, 'yyyy-MM-dd');
      case 'currency':
        return this.currencyPipe.transform(Math.abs(controlValue));

      default:
        return this.rxFormControl?.value;
    }
  }

  @Input() controlType: ControlType = 'input';

  @Input() translate = true;

  @Input() rxFormControl?: RxFormControl;

  isRequired(): boolean {
    if (this.rxFormControl && this.rxFormControl.validator) {
      const validator = this.rxFormControl.validator({} as AbstractControl);
      return validator && validator['required'];
    }
    return false;
  }
}
