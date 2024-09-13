import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { FormArray, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { distinctUntilChanged } from 'rxjs';
import { AreaUnits, EstimatedNumberOfPeople, Hazards } from '../../../../model';
import { DrrInputComponent } from '../../../shared/controls/drr-input/drr-input.component';
import { DrrRadioButtonComponent } from '../../../shared/controls/drr-radio-button/drr-radio-button.component';
import { DrrSelectComponent } from '../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import { ImpactedInfrastructureForm, ProjectAreaForm } from '../drif-fp-form';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drif-fp-step-3',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslocoModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    DrrTextareaComponent,
    DrrInputComponent,
    DrrSelectComponent,
    DrrRadioButtonComponent,
  ],
  templateUrl: './drif-fp-step-3.component.html',
  styleUrl: './drif-fp-step-3.component.scss',
})
export class DrifFpStep3Component {
  formBuilder = inject(RxFormBuilder);
  translocoService = inject(TranslocoService);

  @Input() projectAreaForm!: IFormGroup<ProjectAreaForm>;

  unitOptions = Object.values(AreaUnits).map((value) => ({
    value,
    label: this.translocoService.translate(value),
  }));
  estimatedPeopleImpactedOptions = Object.values(EstimatedNumberOfPeople).map(
    (value) => ({
      value,
      label: this.translocoService.translate(value),
    })
  );
  hazardsOptions = Object.values(Hazards).map((value) => ({
    value,
    label: this.translocoService.translate(value),
  }));

  ngOnInit() {
    this.projectAreaForm
      .get('isInfrastructureImpacted')
      ?.valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        const infrastructureImpacted = this.projectAreaForm.get(
          'infrastructureImpacted'
        ) as FormArray;
        if (!value) {
          infrastructureImpacted?.clear();
        } else {
          if (infrastructureImpacted?.length === 0) {
            this.addInfrastructureImpacted();
          }
        }
      });
  }

  hideInfrastructureImpacted() {
    return (
      this.projectAreaForm.get('isInfrastructureImpacted')?.value === false
    );
  }

  getInfrastructureImpacted() {
    return this.projectAreaForm.get('infrastructureImpacted') as FormArray;
  }

  addInfrastructureImpacted() {
    this.getInfrastructureImpacted().push(
      this.formBuilder.formGroup(ImpactedInfrastructureForm)
    );
  }

  removeInfrastructureImpacted(index: number) {
    this.getInfrastructureImpacted().removeAt(index);
  }

  otherHazardSelected() {
    return this.projectAreaForm.get('relatedHazards')?.value?.includes('Other');
  }
}
