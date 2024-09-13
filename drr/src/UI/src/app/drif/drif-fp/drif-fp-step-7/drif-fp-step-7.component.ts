import { AsyncPipe, CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { FormArray, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { YesNoOption } from '../../../../model';
import { DrrChipAutocompleteComponent } from '../../../shared/controls/drr-chip-autocomplete/drr-chip-autocomplete.component';
import {
  DrrRadioButtonComponent,
  RadioOption,
} from '../../../shared/controls/drr-radio-button/drr-radio-button.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import { OptionsStore } from '../../../store/entities.store';
import { PermitsRegulationsAndStandardsForm } from '../drif-fp-form';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drif-fp-step-7',
  standalone: true,
  imports: [
    CommonModule,
    MatChipsModule,
    MatIconModule,
    TranslocoModule,
    MatFormFieldModule,
    MatCheckboxModule,
    FormsModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    AsyncPipe,
    DrrChipAutocompleteComponent,
    DrrRadioButtonComponent,
    DrrTextareaComponent,
  ],
  templateUrl: './drif-fp-step-7.component.html',
  styleUrl: './drif-fp-step-7.component.scss',
})
export class DrifFpStep7Component {
  optionsStore = inject(OptionsStore);

  @Input()
  permitsRegulationsAndStandardsForm!: IFormGroup<PermitsRegulationsAndStandardsForm>;

  categories = this.optionsStore
    .getOptions()
    ?.standards?.()
    ?.map((s) => s.category);

  professionalOptions = this.optionsStore.getOptions()?.professionals?.();
  standardsAcceptableOptions: RadioOption[] = [
    { value: YesNoOption.Yes, label: 'Yes' },
    { value: YesNoOption.No, label: 'No' },
    { value: YesNoOption.NotApplicable, label: 'Not Applicable' },
  ];

  getStandardsInfoArrayControls() {
    return (
      this.permitsRegulationsAndStandardsForm.get('standards') as FormArray
    ).controls;
  }

  getCategoryControl(category: string) {
    const standardInfoArray = this.permitsRegulationsAndStandardsForm.get(
      'standards'
    ) as FormArray;

    const categoryControl = standardInfoArray.controls.filter(
      (c) => c?.get('category')?.value === category
    );

    return categoryControl;
  }

  getStandardsOptions(category: string) {
    return this.optionsStore
      .getOptions()
      ?.standards?.()
      ?.find((s) => s.category === category)?.standards;
  }
}
