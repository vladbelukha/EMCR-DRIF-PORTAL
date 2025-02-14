import { BreakpointObserver } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, Input, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { RxFormBuilder, RxFormControl } from '@rxweb/reactive-form-validators';
import { NgxMaskDirective } from 'ngx-mask';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drr-currency-input',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    NgxMaskDirective,
    TranslocoModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './drr-currency-input.component.html',
  styleUrl: './drr-currency-input.component.scss',
})
export class DrrCurrencyInputComponent {
  formBuilder = inject(RxFormBuilder);
  breakpointObserver = inject(BreakpointObserver);

  isFocused = false;
  isMobile = false;

  @Input() label = '';
  @Input() id = '';
  @Input() min: number = 0;
  @Input() max: number = 0;
  @Input() allowEnabling = false;

  ngOnInit() {
    this.breakpointObserver
      .observe('(min-width: 768px)')
      .subscribe(({ matches }) => {
        this.isMobile = !matches;
      });
  }

  private _formControl = this.formBuilder.control('', []) as RxFormControl;
  @Input()
  set rxFormControl(rxFormControl: any) {
    this._formControl = rxFormControl as RxFormControl;
  }
  get rxFormControl() {
    return this._formControl;
  }

  @Input()
  set disabled(disabled: boolean) {
    disabled ? this.rxFormControl.disable() : this.rxFormControl.enable();
  }

  changeDetector = inject(ChangeDetectorRef);

  ngAfterViewInit() {
    this.changeDetector.detectChanges();
  }

  getMandatoryMark() {
    return !!this.rxFormControl?.validator?.({})?.required ? '*' : '';
  }

  isRequired(): boolean {
    return this.isMobile
      ? false
      : !!this.rxFormControl?.validator?.({})?.required;
  }

  onFocus() {
    this.isFocused = true;
  }

  onBlur() {
    this.isFocused = false;
  }

  enableInput() {
    this.allowEnabling = false;
    this.rxFormControl.enable();
  }
}
