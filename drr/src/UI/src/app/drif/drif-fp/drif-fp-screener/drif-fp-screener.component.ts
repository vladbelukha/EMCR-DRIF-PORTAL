import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { TranslocoModule } from '@ngneat/transloco';
import {
  RxFormBuilder,
  RxReactiveFormsModule,
} from '@rxweb/reactive-form-validators';
import { YesNoOption } from '../../../../model';
import {
  DrrRadioButtonComponent,
  RadioOption,
} from '../../../shared/controls/drr-radio-button/drr-radio-button.component';
import { ScreenerQuestionsForm } from './drif-fp-screener-form';

@Component({
  selector: 'drr-drif-fp-screener',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RxReactiveFormsModule,
    TranslocoModule,
    MatInputModule,
    MatButtonModule,
    MatListModule,
    DrrRadioButtonComponent,
  ],
  templateUrl: './drif-fp-screener.component.html',
  styleUrl: './drif-fp-screener.component.scss',
})
export class DrifFpScreenerComponent {
  formBuilder = inject(RxFormBuilder);

  screenerForm = this.formBuilder.formGroup(ScreenerQuestionsForm);

  foundationWorkCompletedOptions: RadioOption[] = [
    { value: YesNoOption.Yes, label: 'Yes' },
    { value: YesNoOption.No, label: 'No' },
    {
      value: YesNoOption.NotApplicable,
      label: 'Not Applicable (if applying for foundational project)',
    },
  ];

  yesNoNotRequiredOptions: RadioOption[] = [
    { value: YesNoOption.Yes, label: 'Yes' },
    { value: YesNoOption.No, label: 'No' },
    { value: YesNoOption.NotApplicable, label: 'Not Required' },
  ];

  yesNoNotApplicableOptions: RadioOption[] = [
    { value: YesNoOption.Yes, label: 'Yes' },
    { value: YesNoOption.No, label: 'No' },
    { value: YesNoOption.NotApplicable, label: 'Not Applicable' },
  ];

  skip() {}
}
