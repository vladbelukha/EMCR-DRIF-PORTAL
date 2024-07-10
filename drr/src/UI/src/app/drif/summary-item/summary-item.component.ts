import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { RxFormControl } from '@rxweb/reactive-form-validators';

@Component({
  selector: 'drr-summary-item',
  standalone: true,
  imports: [CommonModule, MatInputModule, TranslocoModule],
  templateUrl: './summary-item.component.html',
  styleUrl: './summary-item.component.scss',
})
export class SummaryItemComponent {
  @Input() label?: string;
  @Input() value?: string | null | undefined;
  @Input() rxFormControl?: RxFormControl;

  isRequired(): boolean {
    if (this.rxFormControl && this.rxFormControl.validator) {
      const validator = this.rxFormControl.validator({} as AbstractControl);
      return validator && validator['required'];
    }
    return false;
  }
}
