import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import {
  AbstractControl,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import {
  RxFormControl,
  RxReactiveFormsModule,
} from '@rxweb/reactive-form-validators';
import { YesNoOption } from '../../../../model';
import {
  DrrRadioButtonComponent,
  RadioOption,
} from '../../../shared/controls/drr-radio-button/drr-radio-button.component';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drif-fp-screener-question',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RxReactiveFormsModule,
    MatIconModule,
    DrrRadioButtonComponent,
    TranslocoModule,
  ],
  template: `
    <div *transloco="let t; read: 'screener'">
      <drr-radio-button
        [label]="t(labelKey!)"
        [rxFormControl]="rxFormControl"
        [options]="options!"
      ></drr-radio-button>
      <div
        *ngIf="isNegativeAnswer()"
        style="display: flex; flex-direction: row; align-items: center; margin-bottom: 1rem"
      >
        <mat-icon style="color: #96c0e6;" [inline]="true">info</mat-icon>
        <i style="margin-left: 1rem; flex: 1">{{
          t('negativeAnswers.' + labelKey)
        }}</i>
      </div>
    </div>
  `,
  styles: [``],
})
export class DrifFpScreenerQuestionComponent {
  private _rxFormControl?: RxFormControl;
  @Input() set rxFormControl(formControl: AbstractControl | null) {
    this._rxFormControl = formControl as RxFormControl;
  }
  get rxFormControl(): RxFormControl | undefined {
    return this._rxFormControl;
  }

  @Input() labelKey?: string;

  @Input() options?: RadioOption[];

  isNegativeAnswer() {
    return (
      this.rxFormControl?.value === false ||
      this.rxFormControl?.value === YesNoOption.No
    );
  }
}
