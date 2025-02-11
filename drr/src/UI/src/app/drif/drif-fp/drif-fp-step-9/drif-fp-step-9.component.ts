import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import {
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { distinctUntilChanged } from 'rxjs';
import { IncreasedOrTransferred } from '../../../../model';
import { DrrChipAutocompleteComponent } from '../../../shared/controls/drr-chip-autocomplete/drr-chip-autocomplete.component';
import { DrrRadioButtonComponent } from '../../../shared/controls/drr-radio-button/drr-radio-button.component';
import {
  DrrSelectComponent,
  DrrSelectOption,
} from '../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import { OptionsStore } from '../../../store/options.store';
import { ProjectRisksForm } from '../drif-fp-form';

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
    DrrSelectComponent,
  ],
  templateUrl: './drif-fp-step-9.component.html',
  styleUrl: './drif-fp-step-9.component.scss',
})
export class DrifFpStep9Component {
  optionsStore = inject(OptionsStore);

  @Input()
  projectRisksForm!: IFormGroup<ProjectRisksForm>;

  complexityRiskOptions = this.optionsStore.options.complexityRisks?.();
  readinessRiskOptions = this.optionsStore.options.readinessRisks?.();
  sensitivityRiskOptions = this.optionsStore.options.sensitivityRisks?.();
  capacityRiskOptions = this.optionsStore.options.capacityRisks?.();
  increasedOrTransferredOptions: DrrSelectOption[] = [
    {
      value: IncreasedOrTransferred.Increased,
      label: IncreasedOrTransferred.Increased,
    },
    {
      value: IncreasedOrTransferred.Transferred,
      label: IncreasedOrTransferred.Transferred,
    },
  ];

  ngOnInit() {
    this.projectRisksForm
      .get('complexityRiskMitigated')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        const complexityRisksControl =
          this.projectRisksForm.get('complexityRisks');
        const complexityRiskCommentsControl = this.projectRisksForm.get(
          'complexityRiskComments',
        );
        if (value === false) {
          complexityRisksControl?.reset();
          complexityRisksControl?.clearValidators();
          complexityRiskCommentsControl?.reset();
          complexityRiskCommentsControl?.clearValidators();
        } else {
          complexityRisksControl?.addValidators(Validators.required);
          complexityRiskCommentsControl?.addValidators(Validators.required);
        }
        complexityRisksControl?.updateValueAndValidity();
        complexityRiskCommentsControl?.updateValueAndValidity();
      });

    this.projectRisksForm
      .get('readinessRiskMitigated')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        const readinessRisksControl =
          this.projectRisksForm.get('readinessRisks');
        const readinessRiskCommentsControl = this.projectRisksForm.get(
          'readinessRiskComments',
        );
        if (value === false) {
          readinessRisksControl?.reset();
          readinessRisksControl?.clearValidators();
          readinessRiskCommentsControl?.reset();
          readinessRiskCommentsControl?.clearValidators();
        } else {
          readinessRisksControl?.addValidators(Validators.required);
          readinessRiskCommentsControl?.addValidators(Validators.required);
        }
        readinessRisksControl?.updateValueAndValidity();
        readinessRiskCommentsControl?.updateValueAndValidity();
      });

    this.projectRisksForm
      .get('sensitivityRiskMitigated')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        const sensitivityRisksControl =
          this.projectRisksForm.get('sensitivityRisks');
        const sensitivityRiskCommentsControl = this.projectRisksForm.get(
          'sensitivityRiskComments',
        );
        if (value === false) {
          sensitivityRisksControl?.reset();
          sensitivityRisksControl?.clearValidators();
          sensitivityRiskCommentsControl?.reset();
          sensitivityRiskCommentsControl?.clearValidators();
        } else {
          sensitivityRisksControl?.addValidators(Validators.required);
          sensitivityRiskCommentsControl?.addValidators(Validators.required);
        }
        sensitivityRisksControl?.updateValueAndValidity();
        sensitivityRiskCommentsControl?.updateValueAndValidity();
      });

    this.projectRisksForm
      .get('capacityRiskMitigated')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        const capacityRisksControl = this.projectRisksForm.get('capacityRisks');
        const capacityRiskCommentsControl = this.projectRisksForm.get(
          'capacityRiskComments',
        );
        if (value === false) {
          capacityRisksControl?.reset();
          capacityRisksControl?.clearValidators();
          capacityRiskCommentsControl?.reset();
          capacityRiskCommentsControl?.clearValidators();
        } else {
          capacityRisksControl?.addValidators(Validators.required);
          capacityRiskCommentsControl?.addValidators(Validators.required);
        }
        capacityRisksControl?.updateValueAndValidity();
        capacityRiskCommentsControl?.updateValueAndValidity();
      });

    this.projectRisksForm
      .get('riskTransferMigigated')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        const increasedOrTransferredControl = this.projectRisksForm.get(
          'increasedOrTransferred',
        );
        const increasedOrTransferredCommentsControl = this.projectRisksForm.get(
          'increasedOrTransferredComments',
        );
        if (value === false) {
          increasedOrTransferredControl?.reset();
          increasedOrTransferredControl?.clearValidators();
          increasedOrTransferredCommentsControl?.reset();
          increasedOrTransferredCommentsControl?.clearValidators();
        } else {
          increasedOrTransferredControl?.addValidators(Validators.required);
          increasedOrTransferredCommentsControl?.addValidators(
            Validators.required,
          );
        }
        increasedOrTransferredControl?.updateValueAndValidity();
        increasedOrTransferredCommentsControl?.updateValueAndValidity();
      });
  }

  getFormControl(name: string) {
    return this.projectRisksForm.get(name) as FormControl;
  }
}
