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
import { DrrAlertComponent } from '../../../shared/drr-alert/drr-alert.component';

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
    DrrAlertComponent,
    TranslocoModule,
  ],
  template: `
    <div *transloco="let t; read: 'screener'">
      <drr-radio-button
        [label]="t(labelKey!)"
        [rxFormControl]="rxFormControl"
        [options]="options!"
      ></drr-radio-button>
      <drr-alert
        class="screener-alert"
        *ngIf="isNegativeAnswer()"
        [type]="'info'"
        [message]="t('negativeAnswers.' + labelKey)"
      ></drr-alert>
    </div>
  `,
  styles: [
    `
      .screener-alert {
        display: flex;
        justify-content: flex-start;
        margin-bottom: 0.5rem;
      }
    `,
  ],
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
