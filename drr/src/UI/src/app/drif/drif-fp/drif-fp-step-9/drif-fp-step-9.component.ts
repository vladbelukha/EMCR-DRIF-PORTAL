import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { distinctUntilChanged } from 'rxjs';
import { DrrChipAutocompleteComponent } from '../../../shared/controls/drr-chip-autocomplete/drr-chip-autocomplete.component';
import { DrrRadioButtonComponent } from '../../../shared/controls/drr-radio-button/drr-radio-button.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import { OptionsStore } from '../../../store/options.store';
import { ProjectRisksForm, TransferRisks } from '../drif-fp-form';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drif-fp-step-9',
  standalone: true,
  imports: [
    CommonModule,
    MatInputModule,
    TranslocoModule,
    DrrChipAutocompleteComponent,
    DrrTextareaComponent,
    DrrRadioButtonComponent,
    FormsModule,
    ReactiveFormsModule,
    MatListModule,
  ],
  templateUrl: './drif-fp-step-9.component.html',
  styleUrl: './drif-fp-step-9.component.scss',
})
export class DrifFpStep9Component {
  optionsStore = inject(OptionsStore);

  @Input()
  projectRisksForm!: IFormGroup<ProjectRisksForm>;

  complexityRiskOptions = this.optionsStore.complexityRisks?.();
  readinessRiskOptions = this.optionsStore.readinessRisks?.();
  sensitivityRiskOptions = this.optionsStore.sensitivityRisks?.();
  capacityRiskOptions = this.optionsStore.capacityRisks?.();
  transferRisksOptions = Object.values(TransferRisks);

  ngOnInit() {
    this.projectRisksForm
      .get('complexityRiskMitigated')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        if (value === false) {
          this.projectRisksForm.get('complexityRisks')?.disable();
        } else {
          this.projectRisksForm.get('complexityRisks')?.enable();
        }
      });

    this.projectRisksForm
      .get('readinessRiskMitigated')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        if (value === false) {
          this.projectRisksForm.get('readinessRisks')?.disable();
        } else {
          this.projectRisksForm.get('readinessRisks')?.enable();
        }
      });

    this.projectRisksForm
      .get('sensitivityRiskMitigated')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        if (value === false) {
          this.projectRisksForm.get('sensitivityRisks')?.disable();
        } else {
          this.projectRisksForm.get('sensitivityRisks')?.enable();
        }
      });

    this.projectRisksForm
      .get('capacityRiskMitigated')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        if (value === false) {
          this.projectRisksForm.get('capacityRisks')?.disable();
        } else {
          this.projectRisksForm.get('capacityRisks')?.enable();
        }
      });

    this.projectRisksForm
      .get('riskTransferMigigated')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        if (value === false) {
          this.projectRisksForm.get('transferRisks')?.setValue([]);
          this.projectRisksForm.get('transferRisks')?.disable();
        } else {
          this.projectRisksForm.get('transferRisks')?.enable();
        }
      });
  }

  getFormControl(name: string) {
    return this.projectRisksForm.get(name) as FormControl;
  }
}
