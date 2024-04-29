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
  ],
})
export class DrrInputComponent {
  formBuilder = inject(RxFormBuilder);

  isFocused = false;

  @Input() label = '';
  @Input() id = '';
  @Input() maxlength: number | string = '';
  @Input() type = 'text';

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
    return this.rxFormControl?.value?.length ?? 0;
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

  @HostListener('keypress', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (this.type === 'tel') {
      const pattern = /[0-9]/; // Allow numbers
      let inputChar = String.fromCharCode(event.charCode);

      if (!pattern.test(inputChar)) {
        // Invalid character, prevent input
        event.preventDefault();
      }
    }
  }
}
