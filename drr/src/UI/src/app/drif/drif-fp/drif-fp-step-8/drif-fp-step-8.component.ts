import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { distinctUntilChanged } from 'rxjs';
import { DrrChipAutocompleteComponent } from '../../../shared/controls/drr-chip-autocomplete/drr-chip-autocomplete.component';
import { DrrRadioButtonComponent } from '../../../shared/controls/drr-radio-button/drr-radio-button.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import { OptionsStore } from '../../../store/options.store';
import { ProjectOutcomesForm } from '../drif-fp-form';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drif-fp-step-8',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslocoModule,
    MatInputModule,
    DrrTextareaComponent,
    DrrRadioButtonComponent,
    DrrChipAutocompleteComponent,
  ],
  templateUrl: './drif-fp-step-8.component.html',
  styleUrl: './drif-fp-step-8.component.scss',
})
export class DrifFpStep8Component {
  optionsStore = inject(OptionsStore);

  @Input() projectOutcomesForm!: IFormGroup<ProjectOutcomesForm>;

  costReductionOptions = this.optionsStore.getOptions()?.costReductions?.();
  coBenefitsOptions = this.optionsStore.getOptions()?.coBenefits?.();
  resiliencyOptions = this.optionsStore.getOptions()?.increasedResiliency?.();

  ngOnInit() {
    this.projectOutcomesForm
      .get('futureCostReduction')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        const costReductions = this.projectOutcomesForm.get('costReductions');
        const costReductionComments = this.projectOutcomesForm.get(
          'costReductionComments'
        );
        if (value === true) {
          costReductions?.setValidators(Validators.required);
          costReductionComments?.setValidators(Validators.required);
        } else {
          costReductions?.clearValidators();
          costReductions?.reset();
          costReductionComments?.clearValidators();
          costReductionComments?.reset();
        }
        costReductions?.updateValueAndValidity();
        costReductionComments?.updateValueAndValidity();
      });

    this.projectOutcomesForm
      .get('produceCoBenefits')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        const coBenefits = this.projectOutcomesForm.get('coBenefits');
        const coBenefitComments =
          this.projectOutcomesForm.get('coBenefitComments');
        if (value === true) {
          coBenefits?.setValidators(Validators.required);
          coBenefitComments?.setValidators(Validators.required);
        } else {
          coBenefits?.clearValidators();
          coBenefits?.reset();
          coBenefitComments?.clearValidators();
          coBenefitComments?.reset();
        }
        coBenefits?.updateValueAndValidity();
        coBenefitComments?.updateValueAndValidity();
      });
  }
}
