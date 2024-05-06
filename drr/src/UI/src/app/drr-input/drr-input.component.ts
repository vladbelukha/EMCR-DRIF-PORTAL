import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  HostListener,
  Input,
  inject,
} from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { RxFormBuilder, RxFormControl } from '@rxweb/reactive-form-validators';
import { NgxMaskDirective } from 'ngx-mask';

// TODO: consider splitting this component into smaller dedicated input type components
@Component({
  selector: 'drr-input',
  templateUrl: './drr-input.component.html',
  styleUrl: './drr-input.component.scss',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatInputModule,
    NgxMaskDirective,
  ],
})
export class DrrInputComponent {
  formBuilder = inject(RxFormBuilder);

  isFocused = false;

  @Input() label = '';
  @Input() id = '';
  @Input() maxlength: number = 0;
  @Input() type = 'text';

  get getMaxLength() {
    return this.type === 'tel' || this.type === 'number'
      ? null
      : this.maxlength;
  }

  get numberInputMin() {
    return this.type === 'number' ? 0 : undefined;
  }

  private _formControl = this.formBuilder.control('', []) as RxFormControl;
  @Input()
  set rxFormControl(rxFormControl: any) {
    this._formControl = rxFormControl as RxFormControl;
  }

  get rxFormControl() {
    return this._formControl;
  }

  changeDetector = inject(ChangeDetectorRef);

  ngAfterViewInit() {
    this.changeDetector.detectChanges();
  }

  getCount(): number {
    const inputValue = this.rxFormControl?.value ?? '';

    let count = 0;
    if (this.type === 'number') {
      // remove decimal point so it's not counted for numeric input
      count = inputValue.replace('.', '').length;
    } else {
      count = inputValue.length;
    }

    return count;
  }

  isRequired(): boolean {
    return !!this.rxFormControl?.errors?.required;
  }

  onFocus() {
    this.isFocused = true;
  }

  onBlur() {
    this.isFocused = false;
  }

  getMask() {
    if (this.type === 'tel') {
      return '000-000-0000';
    }
    return '';
  }

  @HostListener('keypress', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (this.type === 'tel') {
      // Allow numbers
      const pattern = /[0-9]/;
      let inputChar = String.fromCharCode(event.charCode);

      if (!pattern.test(inputChar)) {
        // Invalid character, prevent input
        event.preventDefault();
      }
    }

    if (this.type === 'number') {
      let inputChar = String.fromCharCode(event.charCode);

      // number input doesn't not support maxlength by default
      // so we need to add it manually to match text input behavior,
      // but allow decimal point to be moved around
      if (
        this.maxlength
          ? this.getCount() >= this.maxlength && inputChar !== '.'
          : false
      ) {
        event.preventDefault();
      }

      // Allow positive numbers and decimals
      const pattern = /[0-9.]/;

      if (!pattern.test(inputChar)) {
        // Invalid character, prevent input
        event.preventDefault();
      }
    }
  }
}
