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
        if (value) {
          this.projectOutcomesForm
            .get('costReductions')
            ?.setValidators(Validators.required);
          this.projectOutcomesForm.get('costReductions')?.enable();
          this.projectOutcomesForm
            .get('costReductionComments')
            ?.setValidators(Validators.required);
        } else {
          this.projectOutcomesForm.get('costReductions')?.clearValidators();
          this.projectOutcomesForm.get('costReductions')?.disable();
          this.projectOutcomesForm
            .get('costReductionComments')
            ?.clearValidators();
        }
        this.projectOutcomesForm
          .get('costReductions')
          ?.updateValueAndValidity();
        this.projectOutcomesForm
          .get('costReductionComments')
          ?.updateValueAndValidity();
      });

    this.projectOutcomesForm
      .get('produceCoBenefits')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        if (value) {
          this.projectOutcomesForm
            .get('coBenefits')
            ?.setValidators(Validators.required);
          this.projectOutcomesForm.get('coBenefits')?.enable();
          this.projectOutcomesForm
            .get('coBenefitComments')
            ?.setValidators(Validators.required);
        } else {
          this.projectOutcomesForm.get('coBenefits')?.clearValidators();
          this.projectOutcomesForm.get('coBenefits')?.disable();
          this.projectOutcomesForm.get('coBenefitComments')?.clearValidators();
        }
        this.projectOutcomesForm.get('coBenefits')?.updateValueAndValidity();
        this.projectOutcomesForm
          .get('coBenefitComments')
          ?.updateValueAndValidity();
      });
  }
}
